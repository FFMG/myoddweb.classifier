#include "stdafx.h"
#include "ClassifyEngine.h"
#include <stdexcept>

#include <msclr\marshal_cppstd.h>
#include <stdexcept>

using namespace msclr::interop;

using namespace System;
using namespace System::Diagnostics;
using namespace System::IO;

const unsigned int MAX_CONFIGVALUE_LEN = 1024;

ClassifyEngine::ClassifyEngine() : 
  _hGetProcIDDLL(nullptr),
  _canUseEventLog( false )
{
  //  check if we are allowed/able to use the event log.
  SetCanUseLog();
}

void ClassifyEngine::Release()
{
  // clean up if need be.
  if (nullptr == _hGetProcIDDLL)
  {
    //  we don't try and create the instance if it was never needed.
    return;
  }

  f_Release funci = (f_Release)GetUnmanagedFunction( ProcType::procRelease );

  // did we manage to find the function?
  if (nullptr != funci)
  {
    try
    {
      funci();
    }
    catch (...)
    {
      LogEventError("The function Classifier.Engine 'Release()' threw an exception, please check the logs for more info.");
    }
  }
  else
  {
    //  we need to log the error, but we must still free the library.
    LogEventError("Could not locate the Classifier.Engine 'Release()' function");
  }

  // we can now free everything.
  FreeLibrary(_hGetProcIDDLL);
  _hGetProcIDDLL = nullptr;
}

/**
 * Check if we can use event log or not.
 * @return bool
 */
bool ClassifyEngine::CanUseLog() const 
{
  return _canUseEventLog;
};

/** 
 * Check and set if we can use the event log or not.
 */
void ClassifyEngine::SetCanUseLog()
{
  try
  {
    System::String^ source = gcnew System::String(_eventViewSource.c_str());
    if (!System::Diagnostics::EventLog::SourceExists(source))
    {
      _canUseEventLog = false;
      return;
    }
  }
  catch (System::Security::SecurityException^)
  {
    _canUseEventLog = false;
    return;
  }
  _canUseEventLog = true;
}

void ClassifyEngine::LogEventWarning(String^ sEvent)
{
  if (!CanUseLog())
  {
    return;
  }

  System::String^ source = gcnew System::String(_eventViewSource.c_str());
  System::Diagnostics::EventLog^ appLog = gcnew System::Diagnostics::EventLog();
  appLog->Source = gcnew System::String(_eventViewSource.c_str());
  appLog->WriteEntry(sEvent, EventLogEntryType::Warning);
}

void ClassifyEngine::LogEventError(String^ sEvent)
{
  if (!CanUseLog())
  {
    return;
  }

  System::String^ source = gcnew System::String(_eventViewSource.c_str());
  System::Diagnostics::EventLog^ appLog = gcnew System::Diagnostics::EventLog();
  appLog->Source = gcnew System::String(_eventViewSource.c_str());
  appLog->WriteEntry(sEvent, EventLogEntryType::Error);
}

void ClassifyEngine::LogEventInfo(String^ sEvent)
{
  if (!CanUseLog())
  {
    return;
  }

  System::String^ source = gcnew System::String(_eventViewSource.c_str());
  System::Diagnostics::EventLog^ appLog = gcnew System::Diagnostics::EventLog();
  appLog->Source = source;
  appLog->WriteEntry(sEvent, EventLogEntryType::Information);
}

/**
 * Get an unmanaged function
 * We will throw if the function does not exist. But this should never happen 
 * As we check for all the functions at Load time.
 * @param ProcType procType the function id we are looking for.
 * @return const FARPROC the proc
 */
const FARPROC ClassifyEngine::GetUnmanagedFunction( ProcType procType) const
{
  //  check if we have already loaded this function.
  const auto it = _farProcs.find(procType);
  if (it == _farProcs.end())
  {
    throw std::exception("Could not locate proc, was 'Initialise()' called?");
  }
  return it->second;
}

/***
 * Get or create the unmanaged instance.
 * @param String^ enginePath the path to the engine dll.
 * @return bool
 */
bool ClassifyEngine::InitialiseUnmanagedHinstance(String^ enginePath)
{
  //  do we have it already?
  if (nullptr != _hGetProcIDDLL)
  {
    // why are we trying to re-initialise the engine?
    LogEventWarning("Trying to re-initialise the engine.");

    // free the library and clear the value.
    FreeLibrary(_hGetProcIDDLL);
    _hGetProcIDDLL = nullptr;
  }

  // try and load the library
  if ( enginePath->Length == 0)
  {
    LogEventError("The engine path is empty and not valid.");
    return false;
  }

  // does it exist?
  if (!File::Exists(enginePath))
  {
    LogEventError(String::Format("Unable to locate the given engine '{0}'", enginePath));
    return false;
  }

  // load the library
  auto std_enginePath = marshal_as<std::string>(enginePath);
  _hGetProcIDDLL = LoadLibraryA(std_enginePath.c_str() );
  if (nullptr == _hGetProcIDDLL)
  {
    //  could not load the dll.
    LogEventError("Could not load the Classifier.Engine.dll, is it in the correct location?");
    return false;
  }

  // we are done
  return true;
}

/***
 * Inititalize the c++ plugins.
 * @param String^ eventViewSource the source name we will be using in the event viewer.
 * @param String^ enginePath the path to the dll engine.
 * @param String^ databasePath the path to the database we want to use.
 * @return boolean success or not.
 */
bool ClassifyEngine::Initialise(String^ eventViewSource, String^ enginePath, String^ databasePath)
{
  // set the event view first as might log things.
  _eventViewSource = marshal_as<std::string>(eventViewSource);

  // try and load the dll
  if (!InitialiseUnmanagedHinstance(enginePath))
  {
    return false;
  }

  // prepare all the unmanaged functions.
  if (!InitialiseUnmanagedFunctions( ))
  {
    return false;
  }

  // call the unmanaged function now and return the result for it
  return CallUnmanagedInitialise( databasePath );
}

/**
 * Call the unmanaged 'initialise' function, should only happen once.
 * @param String^ databasePath the database we will be using.
 * @return boolean
 */
bool ClassifyEngine::CallUnmanagedInitialise( String^ databasePath)
{
  // nowe we can try and do the actuall initialize call.
  try
  {
    // the initialise function.
    f_Initialise funci = (f_Initialise)GetUnmanagedFunction( ProcType::procInitialise );

    // did it work?
    if (nullptr == funci)
    {
      LogEventWarning("Could not locate the Classifier.Engine 'Initialise()' function?");
      return false;
    }

    // call the actual function.
    bool resultOfFunctionCall = funci( marshal_as<std::string>(databasePath).c_str() );

    // log that it worked.
    LogEventInfo("Classifier.Engine.dll has been initialised");

    // return the result of the function call.
    return resultOfFunctionCall;
  }
  catch (Exception^ ex )
  {
    //  there was a problem loading this here.
    LogEventWarning(ex->Message);
  }

  // if we are here, then we did not manage to get the data.
  return false;
}

/**
 * Load a single unmanaged function and add it to the list.
 * We will throw if the function does not exist.
 * @return boolean success or not.
 */
bool ClassifyEngine::InitialiseUnmanagedFunction(HINSTANCE hInstance, ProcType procType)
{
  FARPROC procAddress = nullptr;
  switch (procType)
  {
  case procRelease:
    procAddress = GetProcAddress(hInstance, "Release");
    break;

  case procInitialise:
    procAddress = GetProcAddress(hInstance, "Initialise");
    break;

  case procSetConfig:
    procAddress = GetProcAddress(hInstance, "SetConfig");
    break;

  case procGetConfig:
    procAddress = GetProcAddress(hInstance, "GetConfig");
    break;

  case procTrainEx:
    procAddress = GetProcAddress(hInstance, "TrainEx");
    break;

  case procUnTrainEx:
    procAddress = GetProcAddress(hInstance, "UnTrainEx");
    break;

  case procCategorize:
    procAddress = GetProcAddress(hInstance, "Categorize");
    break;

  case procCategorizeWithInfo:
    procAddress = GetProcAddress(hInstance, "CategorizeWithInfo");
    break;

  case procGetCategoryFromUniqueId:
    procAddress = GetProcAddress(hInstance, "GetCategoryFromUniqueId");
    break;

  case procGetCategory:
    procAddress = GetProcAddress(hInstance, "GetCategory");
    break;

  case procGetCategoryInfo:
    procAddress = GetProcAddress(hInstance, "GetCategoryInfo");
    break;

  case procRenameCategory:
    procAddress = GetProcAddress(hInstance, "RenameCategory");
    break;

  case procDeleteCategory:
    procAddress = GetProcAddress(hInstance, "DeleteCategory");
    break;

  case procCreateMagnet:
    procAddress = GetProcAddress(hInstance, "CreateMagnet");
    break;

  case procUpdateMagnet:
    procAddress = GetProcAddress(hInstance, "UpdateMagnet");
    break;

  case procDeleteMagnet:
    procAddress = GetProcAddress(hInstance, "DeleteMagnet");
    break;

  case procGetMagnets:
    procAddress = GetProcAddress(hInstance, "GetMagnets");
    break;

  case procGetVersion:
    procAddress = GetProcAddress(hInstance, "GetVersion");
    break;

  case procLog:
    procAddress = GetProcAddress(hInstance, "Log");
    break;

  case procClearLogEntries:
    procAddress = GetProcAddress(hInstance, "ClearLogEntries");
    break;

  case procGetLogEntries:
    procAddress = GetProcAddress(hInstance, "GetLogEntries");
    break;

  default:
    const auto s = marshal_as<std::string>(
      String::Format("Could not locate the name of the given unmanaged function id. {0}", static_cast<int>(procType))
      );
    throw std::invalid_argument(s);
  }

  // save it, for next time.
  if (procAddress == nullptr)
  {
    return false;
  }

  // add it to our list.
  _farProcs[procType] = procAddress;
  return true;
}

/**
 * Load all the unmaneged functions and make sure that they are placed ub the unordered map
 * @return boolean if anything went wrong.
 */
bool ClassifyEngine::InitialiseUnmanagedFunctions()
{
  if (nullptr == _hGetProcIDDLL)
  {
    LogEventInfo("Trying to load the unmanaged function, but the engine could not be loaded.");
    return false;
  }

  // load all the functions.
  for (int i = ProcType::procFirst; i < ProcType::procLast; ++i)
  {
    // try and load that function
    if (!InitialiseUnmanagedFunction(_hGetProcIDDLL, static_cast<ProcType>(i)))
    {
      // something clearly did not work for one of the functions.
      // maybe we did not map a new value?
      return false;
    }
  }

  // all good.
  return true;
}

bool ClassifyEngine::SetConfig(String^ configName, String^ configValue)
{
  // the initialise function.
  f_SetConfig funci = (f_SetConfig)GetUnmanagedFunction( ProcType::procSetConfig );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'SetConfig()' function?");
    return false;
  }

  // call the function
  std::wstring wConfigName = marshal_as<std::wstring>(configName);
  std::wstring wConfigValue = marshal_as<std::wstring>(configValue);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wConfigName.c_str(), (const char16_t*)wConfigValue.c_str());
}

bool ClassifyEngine::GetConfig(String^ configName, String^% configValue)
{
  //  reset the value
  configValue = "";

  // the initialise function.
  f_GetConfig funci = (f_GetConfig)GetUnmanagedFunction( ProcType::procGetConfig );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetConfig()' function?");
    return false;
  }

  // prepapre to call the function.
  char16_t* wConfigValue = new char16_t[MAX_CONFIGVALUE_LEN + 1];
  memset(wConfigValue, 0, MAX_CONFIGVALUE_LEN);
  
  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  std::wstring wConfigName = marshal_as<std::wstring>(configName).c_str();
  
  // call the function
  bool resultOfCall = funci((const char16_t*)wConfigName.c_str(), wConfigValue, MAX_CONFIGVALUE_LEN);
  if ( true == resultOfCall )
  {
    // this is a 'window' interop and char16_t* is the same size as wchar_t*
    // we simply need to cast the one onto the other to keep the compliler happy.
    const std::wstring ws = (const wchar_t*)wConfigValue;
    configValue = gcnew System::String(ws.c_str());
  }

  // reset the value.
  delete[] wConfigValue;

  // return the result
  return resultOfCall;
}

bool ClassifyEngine::Train(String^ categoryName, String^ textToCategorise, String^ uniqueIdentifier, int weight)
{
  // the initialise function.
  f_TrainEx funci = (f_TrainEx)GetUnmanagedFunction( ProcType::procTrainEx );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'TrainEx()' function?");
    return false;
  }

  // call the function
  std::wstring wCategoryName = marshal_as<std::wstring>(categoryName);
  std::wstring wUniqueIdentifier = marshal_as<std::wstring>(uniqueIdentifier);
  std::wstring wTextToCategorise = marshal_as<std::wstring>(textToCategorise);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wCategoryName.c_str(), 
               (const char16_t*)wTextToCategorise.c_str(),
               (const char16_t*)wUniqueIdentifier.c_str(),
               weight
              );
}

bool ClassifyEngine::UnTrain( String^ uniqueIdentifier, String^ textToCategorise)
{
  // the initialise function.
  f_UnTrainEx funci = (f_UnTrainEx)GetUnmanagedFunction( ProcType::procUnTrainEx );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'UnTrainEx()' function?");
    return false;
  }

  // call the function
  std::wstring wUniqueIdentifier = marshal_as<std::wstring>(uniqueIdentifier);
  std::wstring wTextToCategorise = marshal_as<std::wstring>(textToCategorise);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wUniqueIdentifier.c_str(), 
               (const char16_t*)wTextToCategorise.c_str()
              );
}

/**
 * Categorise a text and return the posible category id, return information as well about the classification.
 * @param String^ textToCategorise the text we want to categorise.
 * @param unsigned int minPercentage the minimum percentage value we want to accept the best possible category, anything lower will not be accepted.
 * @param List<Classifier::Interfaces::WordCategory^> ^% wordsCategory list of words, 'best' category and the percentage.
 * @param Dictionary<int, double> ^% categoryProbabilities the probabilities for each category.
 * @return int the probable category ... or -1
 */
int ClassifyEngine::Categorize(String^ textToCategorise, unsigned int minPercentage, List<Classifier::Interfaces::Helpers::WordCategory^> ^% wordsCategory, Dictionary<int, double> ^% categoryProbabilities )
{
  // sanity check.
  if (wordsCategory == nullptr)
  {
    LogEventError("Trying to categorize with words category, but the array of words has not been initialised!");
    return -1;
  }

  if (categoryProbabilities == nullptr)
  {
    LogEventError("Trying to categorize with categories probabilities, dictionary has not been initialised!");
    return -1;
  }

  // the initialise function.
  const auto funci = (f_CategorizeWithInfo)GetUnmanagedFunction(ProcType::procCategorizeWithInfo);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'CategorizeWithInfo()', function?");
    return -1;
  }

  // call the function
  const auto wTextToCategorise = marshal_as<std::wstring>(textToCategorise);

  // call the category info.
  const auto wordsCategoryInfoLength = 255;
  const auto categoryProbabilityLength = 255;

  auto wordsCategoryInfo = CreateWordCategoryInfo(wordsCategoryInfoLength);
  auto categoryProbability = CreateCategoryProbability( categoryProbabilityLength );

  auto textCategoryInfo = new TextCategoryInfo();
  textCategoryInfo->wordCategoryAndProbability = wordsCategoryInfo;
  textCategoryInfo->wordCategoryAndProbabilityLength = wordsCategoryInfoLength;
  textCategoryInfo->category = categoryProbability;
  textCategoryInfo->categoryLength = categoryProbabilityLength;

  const int overallCategoryId = funci(
    (const char16_t*)wTextToCategorise.c_str(), 
    minPercentage, 
    textCategoryInfo
    );

  // reset our own list in case the user passed something.
  wordsCategory->Clear();

  // add them all to the list.
  for ( auto i = 0; i < wordsCategoryInfoLength; ++i )
  {
    const auto& word = wordsCategoryInfo[i];
    if (word.category == -1)
    {
      break;
    }
    // create the category.
    Classifier::Interfaces::Helpers::WordCategory^ wordCategory = gcnew Classifier::Interfaces::Helpers::WordCategory();

    // re-create it in managed c++ so we can pass it along.
    const std::wstring ws = (const wchar_t*)word.word.word;
    wordCategory->Word = gcnew System::String(ws.c_str());
    wordCategory->Category = word.category;
    wordCategory->Probability = word.probability;

    // add it to the list.
    wordsCategory->Add(wordCategory);
  }

  // reset the probabilities in case the user passed something
  categoryProbabilities->Clear();

  // and copy the values.
  for (auto i = 0; i < categoryProbabilityLength; ++i )
  {
    const auto& category = categoryProbability[i];
    if (category.category == -1)
    {
      //  we are done;
      break;
    }
    categoryProbabilities->Add(category.category, category.probability);
  }

  FreeWordCategoryInfo( wordsCategoryInfo, wordsCategoryInfoLength );
  FreeCategoryProbability(categoryProbability, categoryProbabilityLength );
  delete textCategoryInfo;

  // return the overall category.
  return overallCategoryId;
}

int ClassifyEngine::Categorize(String^ textToCategorise, unsigned int minPercentage)
{
  // the initialise function.
  const auto funci = (f_Categorize)GetUnmanagedFunction( ProcType::procCategorize );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'Categorize()' function?");
    return -1;
  }

  // call the function
  const auto wTextToCategorise = marshal_as<std::wstring>(textToCategorise);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wTextToCategorise.c_str(), minPercentage );
}

int ClassifyEngine::GetCategoryFromUniqueId(String^ uniqueIdentifier)
{
  // the initialise function.
  f_GetCategoryFromUniqueId funci = (f_GetCategoryFromUniqueId)GetUnmanagedFunction( ProcType::procGetCategoryFromUniqueId );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetCategoryFromUniqueId()' function?");
    return -1;
  }

  // call the function
  std::wstring wUniqueIdentifier = marshal_as<std::wstring>(uniqueIdentifier);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wUniqueIdentifier.c_str());
}

int ClassifyEngine::GetCategory(String^ categoryName)
{
  // the initialise function.
  f_GetCategory funci = (f_GetCategory)GetUnmanagedFunction( ProcType::procGetCategory );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetCategory()' function?");
    return -1;
  }

  // call the function
  std::wstring wCategoryName = marshal_as<std::wstring>(categoryName);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wCategoryName.c_str());
}

int ClassifyEngine::GetCategories(Dictionary<int, String^> ^% categories)
{
  // sanity check.
  if (categories == nullptr)
  {
    LogEventError("Trying to get the categories, but the array of categories has not been initialised!");
    return -1;
  }

  // the initialise function.
  f_GetCategoryInfo funci = (f_GetCategoryInfo)GetUnmanagedFunction( ProcType::procGetCategoryInfo );

  // did it work?`
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetCategories()' function?");
    return -1;
  }

  // clear what we have now.
  categories->Clear();
  
  for(auto number = 0;; ++number )
  {
    int id = 0;
    int categoryNameLength = funci(number, id, -1, nullptr);
    if (-1 == categoryNameLength)
    {
      break;
    }

    char16_t* categoryName = new char16_t[categoryNameLength+1];
    memset( categoryName, 0, (categoryNameLength+1)*sizeof(char16_t));
    funci(number, id, categoryNameLength, categoryName);

    // cast the wide string to a char16_t* for windows.
    // this is the same size/value so no work needs to be done here.
    const std::wstring ws = (const wchar_t*)categoryName;
    categories->Add(id, gcnew System::String(ws.c_str()) );

    // we are done with this
    delete [] categoryName;
    categoryName = nullptr;
  }

  // return the number of items.
  return categories->Count;
}

bool ClassifyEngine::RenameCategory(String^ oldName, String^ newName)
{
  // the initialise function.
  f_RenameCategory funci = (f_RenameCategory)GetUnmanagedFunction( ProcType::procRenameCategory );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'RenameCategory()' function?");
    return false;
  }

  // call the function
  std::wstring wOldName = marshal_as<std::wstring>(oldName);
  std::wstring wNewName = marshal_as<std::wstring>(newName);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wOldName.c_str(), (const char16_t*)wNewName.c_str() );
}

bool ClassifyEngine::DeleteCategory(String^ categoryName)
{
  // the initialise function.
  f_DeleteCategory funci = (f_DeleteCategory)GetUnmanagedFunction( ProcType::procDeleteCategory );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'DeleteCategory()' function?");
    return false;
  }

  // call the function
  std::wstring wCategoryName = marshal_as<std::wstring>(categoryName);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wCategoryName.c_str());
}

int ClassifyEngine::CreateMagnet(String^ magnetName, int ruleType, int categoryTarget)
{
  f_CreateMagnet funci = (f_CreateMagnet)GetUnmanagedFunction(ProcType::procCreateMagnet);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'CreateMagnet()' function?");
    return -1;
  }

  // convert to wsstring
  std::wstring wMagnetName = marshal_as<std::wstring>(magnetName);

  // call the function
  return funci((const char16_t*)wMagnetName.c_str(), ruleType, categoryTarget );
}

bool ClassifyEngine::UpdateMagnet(int id, String^ magnetName, int ruleType, int categoryTarget)
{
  f_UpdateMagnet funci = (f_UpdateMagnet)GetUnmanagedFunction(ProcType::procUpdateMagnet);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'UpdateMagnet()' function?");
    return false;
  }

  // convert to wsstring
  std::wstring wMagnetName = marshal_as<std::wstring>(magnetName);

  // call the function
  return funci(id, (const char16_t*)wMagnetName.c_str(), ruleType, categoryTarget );
}

bool ClassifyEngine::DeleteMagnet(int id)
{
  f_DeleteMagnet funci = (f_DeleteMagnet)GetUnmanagedFunction(ProcType::procDeleteMagnet);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'DeleteMagnet()' function?");
    return false;
  }

  // call the function
  return funci( id );
}

int ClassifyEngine::GetMagnets(List<Classifier::Interfaces::Helpers::Magnet^> ^% magnets )
{
  // whatever happens, empty the list.
  magnets = gcnew List<Classifier::Interfaces::Helpers::Magnet^>();

  const auto funci = (f_GetMagnets)GetUnmanagedFunction(ProcType::procGetMagnets);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetMagnets()' function?");
    return -1;
  }

  try
  {
    // first get how many magnets we need
    const auto numberOfMagnets = funci(nullptr, -1 );
    if (numberOfMagnets == -1)
    {
      //  nothing was found.
      return -1;
    }

    // create the magnets
    auto magnetsInfo = CreateMagnets( numberOfMagnets );
    funci( magnetsInfo, numberOfMagnets );
    PrepareMagnetsName( magnetsInfo, numberOfMagnets );
    funci(magnetsInfo, numberOfMagnets);

    // add them all to the list.
    for ( auto i = 0; i < numberOfMagnets; ++i )
    {
      const auto& magnetInfo = magnetsInfo[i];
      auto magnet = gcnew Classifier::Interfaces::Helpers::Magnet();
      magnet->Id = magnetInfo.id;
      magnet->Rule = magnetInfo.ruleType;
      magnet->Category = magnetInfo.categoryTarget;

      // that name.
      const std::wstring ws = (const wchar_t*)magnetInfo.magnetName.word;
      magnet->Name = gcnew System::String(ws.c_str());

      // add it to the list.
      magnets->Add(magnet);
    }

    // free it all
    FreeMagnets( magnetsInfo, numberOfMagnets );

    // return what we found.
    return numberOfMagnets;
  }
  catch (Exception^)
  {
    return 0;
  }
}

/**
 * Get the engine version number.
 * @return int the engine version number.
 */
int ClassifyEngine::GetEngineVersion()
{
  f_GetVersion funci = (f_GetVersion)GetUnmanagedFunction(ProcType::procGetVersion);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetVersion()' function?");
    return -1;
  }

  // just call the funtion.
  return funci();
}

/**
 * Log a message to the database
 * @param Unique to the souce, something like "myapp.information", max 255 chars</param>
 * @param The entry we are logging, max 1024 chars.</param>
 * @return in the entry id number.
 */
int ClassifyEngine::Log(String^ source, String^ entry)
{
  f_Log funci = (f_Log)GetUnmanagedFunction(ProcType::procLog);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'Log()' function?");
    return -1;
  }

  // call the function
  std::wstring wSource = marshal_as<std::wstring>(source);
  std::wstring wEntry = marshal_as<std::wstring>(entry);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wSource.c_str(), (const char16_t*)wEntry.c_str());
}

/**
 * Clear log entries that are older than a certain date
 * @param olderThan the date we want to delte.
 * @return success or not
 */
bool ClassifyEngine::ClearLogEntries(long long olderThan)
{
  f_ClearLogEntries funci = (f_ClearLogEntries)GetUnmanagedFunction(ProcType::procClearLogEntries);

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'ClearLogEntries()' function?");
    return false;
  }

  // try and clear the data
  return funci( olderThan );
}

int ClassifyEngine::GetLogEntries(List<Classifier::Interfaces::Helpers::LogEntry^> ^% entries, int max)
{
  // whatever happens, empty the list.
  entries = gcnew List<Classifier::Interfaces::Helpers::LogEntry^>();

  const auto funci = (f_GetLogEntries)GetUnmanagedFunction(ProcType::procGetLogEntries );

  // did it work?
  if (nullptr == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetLogEntries( ... )' function?");
    return -1;
  }

  // create a log entry
  auto logEntries = CreateLogEntries( max );
  int numberOfEntries = funci(logEntries, max );
  if (numberOfEntries == -1)
  {
    //  nothing was found.
    FreeLogEntries( logEntries, max );
    return -1;
  }
  
  // add them all to the list.
  for (auto i = 0; i < numberOfEntries; ++i)
  {
    auto entry = gcnew Classifier::Interfaces::Helpers::LogEntry();

    const auto& logEntry = logEntries[i];
    entry->Id = logEntry.id;

    const std::wstring wsource = (const wchar_t*)logEntry.source.word;
    entry->Source = gcnew System::String(wsource.c_str());

    const std::wstring wentry = (const wchar_t*)logEntry.entry.word;
    entry->Entry = gcnew System::String(wentry.c_str());

    entry->Unixtime = logEntry.unixtime;

    // add it to the list.
    entries->Add(entry);
  }

  // done with the log entries
  FreeLogEntries( logEntries, max );

  // return what we found.
  return numberOfEntries;
}

LogEntryInfo* ClassifyEngine::CreateLogEntries(int count)
{
  const auto length = 256;

  auto logEntries = new LogEntryInfo[count];
  for (int i = 0; i < count; ++i)
  {
    auto& entry = logEntries[i];
    entry.entry = CreateStringWithLength(length);
    entry.source = CreateStringWithLength(length);
    entry.id = -1;
    entry.unixtime = 0;
  }
  return logEntries;
}

/**
 * \param the entries we are trying to free.
 */
void ClassifyEngine::FreeLogEntries(LogEntryInfo* logEntries, int count )
{
  for (int i = 0; i < count; ++i)
  {
    auto& entry = logEntries[i];
    if (entry.entry.word != nullptr)
    {
      delete[] entry.entry.word;
    }
    if (entry.source.word != nullptr)
    {
      delete[] entry.source.word;
    }
  }
  delete [] logEntries;
}

/**
 * \param the magnets we are trying to free.
 * \param the number of magnets
 */
void ClassifyEngine::FreeMagnets(MagnetInfo* magnets, int count)
{
  for (int i = 0; i < count; ++i)
  {
    auto& magnet = magnets[i];
    if (magnet.magnetName.word != nullptr)
    {
      delete[] magnet.magnetName.word;
    }
  }
  delete[] magnets;
}

void ClassifyEngine::PrepareMagnetsName(MagnetInfo* magnets, int count)
{
  for (int i = 0; i < count; ++i)
  {
    auto& magnet = magnets[i];
    if (magnet.magnetName.word != nullptr)
    {
      delete[] magnet.magnetName.word;
      magnet.magnetName.word = nullptr;
    }
    magnet.magnetName = CreateStringWithLength(magnet.magnetName.length);
  }
}

MagnetInfo* ClassifyEngine::CreateMagnets(int count)
{
  auto magnets = new MagnetInfo[count];
  for (int i = 0; i < count; ++i)
  {
    auto& magnet = magnets[i];
    magnet.id = -1;
    magnet.categoryTarget = -1;
    magnet.magnetName.word = nullptr;
    magnet.magnetName.length = 0;
    magnet.ruleType = -1;
  }
  return magnets;
}

WordCategoryAndProbability* ClassifyEngine::CreateWordCategoryInfo(int count)
{
  const auto length = 256;
  auto words = new WordCategoryAndProbability[count];
  for (int i = 0; i < count; ++i)
  {
    auto& word = words[i];
    word.category = -1;
    word.probability = 0.0;
    word.word = CreateStringWithLength( length );
  }
  return words;
}

void ClassifyEngine::FreeWordCategoryInfo(WordCategoryAndProbability* words, int size)
{
  for (int i = 0; i < size; ++i)
  {
    auto& word = words[i];
    if (word.word.word == nullptr)
    {
      continue;
    }
    delete [] word.word.word;
  }
  delete [] words;
}

CategoryProbability* ClassifyEngine::CreateCategoryProbability(int size)
{
  auto categories = new CategoryProbability[size];
  for (int i = 0; i < size; ++i)
  {
    auto& category = categories[i];
    category.category = -1;
    category.probability = 0;
  }
  return categories;
}

void ClassifyEngine::FreeCategoryProbability(CategoryProbability* categories, int size)
{
  delete[] categories;
}
