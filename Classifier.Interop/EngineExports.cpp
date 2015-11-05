#include "stdafx.h"
#include "EngineExports.h"
#include "ClassifyEngine.h"

// --------------------------------------------------------------------------------------------------
extern "C" LPVOID WINAPI Engine_GetObject(UINT32 id )
{
  switch (id)
  {
  case IID_IClassify1:
    return new ClassifyEngine();
    break;

  default:
    break;
  }

  return NULL;
}