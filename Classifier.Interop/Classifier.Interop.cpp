// This is the main DLL file.

#include "stdafx.h"
#include "Classifier.Interop.h"
#include "EngineExports.h"

namespace Classifier
{
  namespace Interop
  {
    Classify::Classify() : _iEngine( NULL )
    {
      // use the exported method in the underlying DLL
      _iEngine = (ClassifyEngine*) Engine_GetObject(IID_IClassify1 );
    }

    Classify::~Classify()
    {
      this->!Classify();
    }

    Classify::!Classify()
    {
      Release();
    }

    void Classify::Release()
    {
      if (_iEngine == NULL)
      {
        return;
      }

      _iEngine->Release();
      _iEngine = NULL;
    }

    bool Classify::Initialise(String^ eventViewSource, String^ enginePath, String^ databasePath)
    {
      return _iEngine->Initialise(eventViewSource, enginePath, databasePath);
    }

    bool Classify::SetConfig(String^ configName, String^ configValue)
    {
      return _iEngine->SetConfig(configName, configValue);
    }

    bool Classify::GetConfig(String^ configName, String^% configValue)
    {
      return _iEngine->GetConfig(configName, configValue);
    }

    bool Classify::Train( String^ categoryName, String^ textToCategorise, String^ uniqueIdentifier, int weight )
    {
      return _iEngine->Train(categoryName, textToCategorise, uniqueIdentifier, weight);
    }

    bool Classify::UnTrain( String^ uniqueIdentifier, String^ textToCategorise)
    {
      return _iEngine->UnTrain( uniqueIdentifier, textToCategorise);
    }

    int Classify::Categorize(String^ textToCategorise, unsigned int minPercentage)
    {
      return _iEngine->Categorize(textToCategorise, minPercentage);
    }

    int Classify::Categorize(String^ textToCategorise, unsigned int minPercentage, List<Classifier::Interfaces::Helpers::WordCategory^> ^% wordsCategory, Dictionary<int, double> ^% categoryProbabilities)
    {
      return _iEngine->Categorize(textToCategorise, minPercentage, wordsCategory, categoryProbabilities);
    }

    int Classify::GetCategory(String^ categoryName)
    {
      return _iEngine->GetCategory(categoryName );
    }

    int Classify::GetCategories(Dictionary<int, String^> ^% categories)
    {
      return _iEngine->GetCategories(categories);
    }

    bool Classify::RenameCategory(String^ oldName, String^ newName)
    {
      return _iEngine->RenameCategory(oldName, newName);
    }

    bool Classify::DeleteCategory(String^ categoryName)
    {
      return _iEngine->DeleteCategory(categoryName);
    }

    int Classify::GetCategoryFromUniqueId(String^ uniqueIdentifier)
    {
      return _iEngine->GetCategoryFromUniqueId(uniqueIdentifier);
    }

    int Classify::CreateMagnet(String^ magnetName, int ruleType, int categoryTarget)
    {
      return _iEngine->CreateMagnet(magnetName, ruleType, categoryTarget);
    }

    bool Classify::UpdateMagnet(int id, String^ magnetName, int ruleType, int categoryTarget)
    {
      return _iEngine->UpdateMagnet(id, magnetName, ruleType, categoryTarget);
    }

    bool Classify::DeleteMagnet(int id)
    {
      return _iEngine->DeleteMagnet(id);
    }

    int Classify::GetMagnets(List<Classifier::Interfaces::Helpers::Magnet^> ^% magnets )
    {
      return _iEngine->GetMagnets( magnets );
    }

    //
    // Logs
    //
    int Classify::Log(String^ source, String^ entry)
    {
      return _iEngine->Log(source, entry );
    }

    bool Classify::ClearLogEntries(long long olderThan)
    {
      return _iEngine->ClearLogEntries(olderThan);
    }

    int Classify::GetLogEntries(List<Classifier::Interfaces::Helpers::LogEntry^> ^% entries, int max )
    {
      return _iEngine->GetLogEntries(entries, max);
    }

    //
    // Version
    //
    int Classify::GetEngineVersion()
    {
      return _iEngine->GetEngineVersion();
    }
  }
}