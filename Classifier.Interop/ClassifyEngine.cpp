#include "stdafx.h"
#include "ClassifyEngine.h"

#include <msclr\marshal_cppstd.h>
using namespace msclr::interop;

using namespace System;
using namespace System::Diagnostics;
using namespace System::IO;

const unsigned int MAX_CONFIGVALUE_LEN = 1024;

ClassifyEngine::ClassifyEngine() : 
  _hGetProcIDDLL( NULL ),
  _canUseEventLog( false )
{
  //  check if we are allowed/able to use the event log.
  SetCanUseLog();
}

void ClassifyEngine::Release()
{
  // clean up if need be.
  if (NULL == _hGetProcIDDLL)
  {
    //  we don't try and create the instance if it was never needed.
    return;
  }

  f_Release funci = (f_Release)GetUnmanagedFunction( ProcType::procRelease );

  // did we manage to find the function?
  if (NULL != funci)
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
  _hGetProcIDDLL = NULL;
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

FARPROC ClassifyEngine::GetUnmanagedFunction(ProcType procType)
{
  //  check if we have already loaded this function.
  ProcsFarProc::const_iterator it = _farProcs.find(procType);
  if (it != _farProcs.end())
  {
    return it->second;
  }

  //  get the instance.
  HINSTANCE hInstance = GetUnmanagedHinstance();
  if (NULL == hInstance)
  {
    return NULL;
  }

  FARPROC procAddress = NULL;
  switch (procType)
  {
  case procRelease:
    procAddress = GetProcAddress(_hGetProcIDDLL, "Release");
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

  case procGetCategories:
    procAddress = GetProcAddress(hInstance, "GetCategories");
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

  default:
    break;
  }

  // save it, for next time.
  _farProcs[procType] = procAddress;

  // return what we found.
  return procAddress;
}

/***
 * Get or create the unmanaged instance.
 * @return HINSTANCE the created instance or NULL if it could not be created.
 */
HINSTANCE ClassifyEngine::GetUnmanagedHinstance()
{
  //  do we have it already?
  if (NULL != _hGetProcIDDLL)
  {
    return _hGetProcIDDLL;
  }

  if (_enginePath.length() == 0)
  {
    LogEventWarning("The engine path has not been set?");
    return false;
  }

  // load the library
  _hGetProcIDDLL = LoadLibraryA( _enginePath.c_str() );
  if (NULL == _hGetProcIDDLL)
  {
    //  could not load the dll.
    LogEventError("Could not load the Classifier.Engine.dll, is it in the correct location?");
    return NULL;
  }

  // we are done
  return _hGetProcIDDLL;
}

bool ClassifyEngine::SetUnmanagedEnginePath(String^ enginePath)
{
  // assume that we have nothing
  _enginePath.clear();

  // does this file exist?
  if (!File::Exists(enginePath))
  {
    LogEventError( String::Format( "Unable to locate the given engine '{0}'", enginePath ) );
    return false;
  }

  // set the path to the dll.
  _enginePath = marshal_as<std::string>( enginePath );

  // log the success
  LogEventInfo(String::Format("The engine path has been set '{0}'", enginePath));

  // success
  return true;
}

/***
 * Inititalize the c++ plugins.
 * @param String^ eventViewSource the source name we will be using in the event viewer.
 * @param String^ enginePath the path to the dll engine.
 * @param String^ databasePath the path to the database we want to use.
 * @return boolean success or not.
 */
bool ClassifyEngine::Initialise(String^ eventViewSource, String^ enginePath, String^ databasePath )
{
  // set the event view first as might log things.
  _eventViewSource = marshal_as<std::string>(eventViewSource);

  // does this file exist?
  if (!SetUnmanagedEnginePath(enginePath))
  {
    return false;
  }

  // nowe we can try and do the actuall initialize call.
  try
  {
    // the initialise function.
    f_Initialise funci = (f_Initialise)GetUnmanagedFunction( ProcType::procInitialise );

    // did it work?
    if (NULL == funci)
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

bool ClassifyEngine::SetConfig(String^ configName, String^ configValue)
{
  // the initialise function.
  f_SetConfig funci = (f_SetConfig)GetUnmanagedFunction( ProcType::procSetConfig );

  // did it work?
  if (NULL == funci)
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
  if (NULL == funci)
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
  if (NULL == funci)
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
  if (NULL == funci)
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

int ClassifyEngine::Categorize(String^ textToCategorise, unsigned int minPercentage, List<Classifier::Interfaces::WordCategory^> ^% wordsCategory)
{
  // sanity check.
  if (wordsCategory == nullptr)
  {
    LogEventError("Trying to categorize with words category, but the array of words has not been initialised!");
    return -1;
  }

  // the initialise function.
  f_CategorizeWithWordCategory funci = (f_CategorizeWithWordCategory)GetUnmanagedFunction(ProcType::procCategorizeWithInfo);

  // did it work?
  if (NULL == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'CategorizeWithInfo()', function?");
    return -1;
  }

  // call the function
  std::wstring wTextToCategorise = marshal_as<std::wstring>(textToCategorise);

  // call the category info.
  wordscategory_info wordsCategoryInfo;
  int overallCategoryId = funci((const char16_t*)wTextToCategorise.c_str(), minPercentage, wordsCategoryInfo );

  // add them all to the list.
  for (wordscategory_info::const_iterator it = wordsCategoryInfo.begin();
    it != wordsCategoryInfo.end();
    ++it)
  {
    // create the category.
    Classifier::Interfaces::WordCategory^ wordCategory = gcnew Classifier::Interfaces::WordCategory();

    // re-create it in managed c++ so we can pass it along.
    const std::wstring ws = (const wchar_t*)it->first.c_str();
    wordCategory->Word = gcnew System::String(ws.c_str());
    wordCategory->Category = it->second.category;
    wordCategory->Probability = it->second.probability;

    // add it to the list.
    wordsCategory->Add(wordCategory);
  }

  // return the overall category.
  return overallCategoryId;
}

int ClassifyEngine::Categorize(String^ textToCategorise, unsigned int minPercentage)
{
  // the initialise function.
  f_Categorize funci = (f_Categorize)GetUnmanagedFunction( ProcType::procCategorize );

  // did it work?
  if (NULL == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'Categorize()' function?");
    return -1;
  }

  // call the function
  std::wstring wTextToCategorise = marshal_as<std::wstring>(textToCategorise);

  // cast the wide string to a char16_t* for windows.
  // this is the same size/value so no work needs to be done here.
  return funci((const char16_t*)wTextToCategorise.c_str(), minPercentage );
}

int ClassifyEngine::GetCategoryFromUniqueId(String^ uniqueIdentifier)
{
  // the initialise function.
  f_GetCategoryFromUniqueId funci = (f_GetCategoryFromUniqueId)GetUnmanagedFunction( ProcType::procGetCategoryFromUniqueId );

  // did it work?
  if (NULL == funci)
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
  if (NULL == funci)
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
  f_GetCategories funci = (f_GetCategories)GetUnmanagedFunction( ProcType::procGetCategories );

  // did it work?
  if (NULL == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetCategories()' function?");
    return -1;
  }

  // clear what we have now.
  categories->Clear();
  
  // call the function
  std::map<int, std::u16string> u16categories;
  int result = funci(u16categories);
  if (result <= 0)
  {
    // either an error or nothing at all.
    return result;
  }

  // we now need to recreate the result.
  for (std::map<int, std::u16string>::const_iterator it = u16categories.begin();
       it != u16categories.end();
       ++it)
  {
    // cast the wide string to a char16_t* for windows.
    // this is the same size/value so no work needs to be done here.
    const std::wstring ws = (const wchar_t*)it->second.c_str();
    categories->Add(it->first, gcnew System::String(ws.c_str()) );
  }

  // return the result.
  return result;
}

bool ClassifyEngine::RenameCategory(String^ oldName, String^ newName)
{
  // the initialise function.
  f_RenameCategory funci = (f_RenameCategory)GetUnmanagedFunction( ProcType::procRenameCategory );

  // did it work?
  if (NULL == funci)
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
  if (NULL == funci)
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
  if (NULL == funci)
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
  if (NULL == funci)
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
  if (NULL == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'DeleteMagnet()' function?");
    return false;
  }

  // call the function
  return funci( id );
}

int ClassifyEngine::GetMagnets(List<Classifier::Interfaces::Magnet^> ^% magnets )
{
  // whatever happens, empty the list.
  magnets = gcnew List<Classifier::Interfaces::Magnet^>();

  f_GetMagnets funci = (f_GetMagnets)GetUnmanagedFunction(ProcType::procGetMagnets);

  // did it work?
  if (NULL == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetMagnets()' function?");
    return -1;
  }

  magnets_info magnetsInfo;
  int numberOfMagnets = funci(magnetsInfo);
  if (numberOfMagnets == -1)
  {
    //  nothing was found.
    return -1;
  }

  // add them all to the list.
  for (magnets_info::const_iterator it = magnetsInfo.begin();
       it != magnetsInfo.end();
       ++it)
  {
    Classifier::Interfaces::Magnet^ magnet = gcnew Classifier::Interfaces::Magnet();
    magnet->Id = it->first;
    magnet->Rule = it->second.ruleType;
    magnet->Category = it->second.categoryTarget;

    // that name.
    const std::wstring ws = (const wchar_t*)it->second.magnetName.c_str();
    magnet->Name = gcnew System::String(ws.c_str());

    // add it to the list.
    magnets->Add(magnet);
  }

  // return what we found.
  return numberOfMagnets;
}

/**
 * Get the engine version number.
 * @return int the engine version number.
 */
int ClassifyEngine::GetEngineVersion()
{
  f_GetVersion funci = (f_GetVersion)GetUnmanagedFunction(ProcType::procGetVersion);

  // did it work?
  if (NULL == funci)
  {
    LogEventWarning("Could not locate the Classifier.Engine 'GetVersion()' function?");
    return -1;
  }

  // just call the funtion.
  return funci();
}
