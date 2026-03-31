import { AppSettings } from 'src/app/share/values/app-settings';
import { isNullOrWhitespace } from '../../share/helpers/is-null-or-white-space';
import { SearchAlgorithms, SearchData } from '../services/glossary-search-message-bus.service';

export function filterGlossary<T extends GlossaryHeaderSegment>(glossary: T[], searchData: SearchData, isDemoMode: boolean): T[] {
  const { searchText, algorithm } = searchData;
  var result: T[] = [];
  switch (algorithm) {
    case SearchAlgorithms.Unknown:
      result =  glossary;
      break;

    case SearchAlgorithms.HeaderContains:
      if (isNullOrWhitespace(searchText)) {
        result = glossary;
      }
      else {
        result = glossary.filter(
          term => term.header?.toLowerCase().includes(searchText.toLowerCase()) === true
        ).sort((a, b) => a.header.localeCompare(b.header));
      }
      break;

    case SearchAlgorithms.StartFromLetter:
      if (isNullOrWhitespace(searchText)) {
        result = glossary;
      }
      else {
        result = glossary.filter(
          term => term.header?.substring(0, 1)?.toLowerCase() === searchText.toLowerCase()
        ).sort((a, b) => a.header.localeCompare(b.header));
      }
      break;
  }

  if(isDemoMode)
    result.forEach(
      term => {
        if(!AppSettings.freeTrialGlossaryLetters.some( l => term.header?.substring(0, 1)?.toLowerCase() === l)){
          term.isDisabled = true;
        }
      }
    );
  return result;
}

export interface GlossaryHeaderSegment {
  header: string;
  isDisabled: boolean;
}

