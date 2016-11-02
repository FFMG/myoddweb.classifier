// Classifier.Interop.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

#include "ClassifyEngine.h"

namespace Classifier
{
  namespace Interop
  {
    public ref class Classify : public Classifier::Interfaces::IClassify1
    {
    public:
      Classify();
      virtual ~Classify();

    protected:
      !Classify();

    public:
      //
      // Version
      //
      virtual int GetEngineVersion();

      //
      //  Setup
      //
      virtual bool Initialise(String^ eventViewSource, String^ enginePath, String^ databasePath);
      virtual void Release();

      //
      //  Configurations 
      //
      virtual bool SetConfig(String^ configName, String^ configValue);
      virtual bool GetConfig(String^ configName, String^% configValue);

      //
      //  Classification 
      //
      virtual bool Train( String^ categoryName, String^ textToCategorise, String^ uniqueIdentifier, int weight );
      virtual bool UnTrain( String^ uniqueIdentifier, String^ textToCategorise);
      virtual int Categorize( String^ textToCategorise, unsigned int minPercentage);
      virtual int Categorize(String^ textToCategorise, unsigned int minPercentage, List<Classifier::Interfaces::WordCategory^> ^% wordsCategory, Dictionary<int, double> ^% categoryProbabilities);

      //
      //  Information / Manipulation
      //
      virtual int GetCategoryFromUniqueId(String^ uniqueIdentifier);
      virtual int GetCategory( String^ categoryName);
      virtual int GetCategories( Dictionary<int, String^> ^% categories);
      virtual bool RenameCategory(String^ oldName, String^ newName);
      virtual bool DeleteCategory(String^ categoryName);

      //
      //  Magnets
      //
      virtual int CreateMagnet(String^ magnetName, int ruleType, int categoryTarget);
      virtual bool UpdateMagnet(int id, String^ magnetName, int ruleType, int categoryTarget);
      virtual bool DeleteMagnet(int id);
      virtual int GetMagnets(List<Classifier::Interfaces::Magnet^> ^% magnets );

    protected:
      ClassifyEngine* _iEngine;
    };
  }
}
