#pragma once

#include <string>
#include <map>
#include <unordered_map>

using namespace System::Collections::Generic;

using namespace System;

class ClassifyEngine
{
public:
  ClassifyEngine();

public:
  //
  // Version
  //
  int GetEngineVersion();

  //
  //  Setup
  //
  bool Initialise(String^ eventViewSource, String^ configurationXml, String^ databasePath);
  void Release();

  //
  //  Configurations 
  //
  bool SetConfig(String^ configName, String^ configValue);
  bool GetConfig(String^ configName, String^% configValue);

  //
  //  Classification 
  //
  bool Train(String^ categoryName, String^ textToCategorise, String^ uniqueIdentifier, int weight );
  bool UnTrain( String^ uniqueIdentifier, String^ textToCategorise);
  int Categorize(String^ textToCategorise, unsigned int minPercentage);
  int Categorize(String^ textToCategorise, unsigned int minPercentage, List<Classifier::Interfaces::Helpers::WordCategory^> ^% wordsCategory, Dictionary<int, double> ^% categoryProbabilities );

  //
  //  Information / Manipulation
  //
  int GetCategoryFromUniqueId(String^ uniqueIdentifier);
  int GetCategory(String^ categoryName);
  int GetCategories(Dictionary<int, String^> ^% categories);
  bool RenameCategory(String^ oldName, String^ newName);
  bool DeleteCategory(String^ categoryName);

  //
  //  Magnets
  // 
  int CreateMagnet(String^ magnetName, int ruleType, int categoryTarget);
  bool UpdateMagnet(int id, String^ magnetName, int ruleType, int categoryTarget);
  bool DeleteMagnet(int id);
  int GetMagnets(List<Classifier::Interfaces::Helpers::Magnet^> ^% magnets );

protected:
  typedef bool(__stdcall *f_Initialise)(const char*);
  typedef bool(__stdcall *f_SetConfig)(const char16_t*, const char16_t*);
  typedef bool(__stdcall *f_GetConfig)(const char16_t*, char16_t*, size_t);
  typedef bool(__stdcall *f_TrainEx)(const char16_t*, const char16_t*, const char16_t*, const int );
  typedef bool(__stdcall *f_UnTrainEx)(const char16_t*, const char16_t*);
  typedef bool(__stdcall *f_RenameCategory)(const char16_t*, const char16_t*);
  typedef bool(__stdcall *f_DeleteCategory)(const char16_t*);
  typedef bool(__stdcall *f_Release)();

  typedef int(__stdcall *f_GetCategories)(std::map<int, std::u16string>&);
  typedef int(__stdcall *f_Categorize)(const char16_t*, unsigned int);
  typedef int(__stdcall *f_GetCategoryFromUniqueId)(const char16_t*);
  typedef int(__stdcall *f_GetCategory)(const char16_t*);

  typedef int(__stdcall *f_CreateMagnet)(const char16_t*, int , int);
  typedef bool(__stdcall *f_UpdateMagnet)(int, const char16_t*, int, int);
  typedef bool(__stdcall *f_DeleteMagnet)(int);

  struct MagnetInfo
  {
    std::u16string magnetName;
    int ruleType;
    int categoryTarget;
  };

  // all the magnets, the id => MagnetInfo
  typedef std::unordered_map<int, MagnetInfo> magnets_info;
  typedef int(__stdcall *f_GetMagnets)(magnets_info&);

  struct WordCategoryInfo
  {
    int category;
    double probability;
  };
  typedef std::unordered_map<std::u16string, WordCategoryInfo> wordscategory_info;
  typedef std::unordered_map<int, double> categoriesProbabilities_info;
  typedef int(__stdcall *f_CategorizeWithWordCategory)(const char16_t*, unsigned int, wordscategory_info&, categoriesProbabilities_info&);
  
  typedef int(__stdcall *f_GetVersion)();

  enum ProcType
  {
    procUnk = 0,
    procInitialise,
    procSetConfig,
    procGetConfig,
    procTrainEx,
    procUnTrainEx,
    procCategorize,
    procCategorizeWithInfo,
    procGetCategoryFromUniqueId,
    procGetCategory,
    procGetCategories,
    procRenameCategory,
    procDeleteCategory,

    procCreateMagnet,
    procDeleteMagnet,
    procUpdateMagnet,
    procGetMagnets,
    procGetVersion,

    procRelease,
  };

  HINSTANCE _hGetProcIDDLL;
  HINSTANCE GetUnmanagedHinstance();
  FARPROC GetUnmanagedFunction(ProcType procType);

  typedef std::unordered_map<ProcType, FARPROC> ProcsFarProc;
  ProcsFarProc _farProcs;

private:
  std::string _enginePath;

  bool SetUnmanagedEnginePath( String^ enginePath );

protected:
  // the name of the event viewer source.
  std::string _eventViewSource;

  //  all the event loggins.
  void LogEventWarning(String^ sEvent);
  void LogEventInfo(String^ sEvent);
  void LogEventError(String^ sEvent);

private:
  bool _canUseEventLog;
  bool CanUseLog() const;
  void SetCanUseLog();
};

