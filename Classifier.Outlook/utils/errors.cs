namespace myoddweb.classifier
{
  public enum Errors
  {
    // general errors 
    //  0 - 99
    None              = 0,      //  do not return this, try and use an actual error/success message
    Success           = 1,

    // Categories 
    //  100 - 199
    CategoryNoEngine  = 100,    //  the engine is missing and-or invalid. 
    CategoryNotFound  = 101,    //  a category id was given, but that id was not found. 
    CategoryTrainning = 102,    //  trainning returned false. 
  }
}
