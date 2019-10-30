#pragma once
#include <string>

struct StringWithLength
{
  char16_t* word;
  int length;
};

static StringWithLength CreateStringWithLength(int length)
{
  auto word = new char16_t[length + 1];
  memset(word, 0, (length + 1) * sizeof(char16_t));

  auto sl = StringWithLength();
  sl.length = length;
  sl.word = word;
  return sl;
}

static int SetStringWithLengthWord(const std::u16string& given, StringWithLength& swl)
{
  const auto actual = (int)given.length() > swl.length ? swl.length : (int)given.length();
  memcpy(swl.word, given.c_str(), actual * sizeof(char16_t));
  return actual;
}

StringWithLength CreateStringWithLength(int length);

//
// Categories
//
// The word, the category and the probability of that word.
struct WordCategoryAndProbability
{
  StringWithLength word;
  int category;             //  most likely category
  double probability;       //  probability of that word category
};
struct CategoryProbability
{
  int category;
  double probability;
};

struct TextCategoryInfo
{
  WordCategoryAndProbability* wordCategoryAndProbability; // array of words in the text and probabilities.
  int wordCategoryAndProbabilityLength;                   // number of words and probabilitites.
  CategoryProbability* category;                          // array of categories and their probabilities
  int categoryLength;                                     // number of categories
};

//
// Magnets
//
struct MagnetInfo
{
  int id;
  StringWithLength magnetName;
  int ruleType;
  int categoryTarget;
};

//
// log info
//
struct LogEntryInfo
{
  int id;
  StringWithLength source;
  StringWithLength entry;
  long long unixtime;
};
