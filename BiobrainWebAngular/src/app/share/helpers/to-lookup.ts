import { OperatorFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { hasValue } from './has-value';

export function toLookup<TItem, TKey>(keyFunc: (i: TItem) => TKey, thisArg?: any): OperatorFunction<TItem[], Map<TKey, TItem[]>> {
  return map(items => items.reduce((lookup, item) => {
    const key = keyFunc(item);
    let list = lookup.get(key);
    if (!hasValue(list)) {
      list = [];
      lookup.set(key, list);
    }

    list.push(item);
    return lookup;
  }, new Map<TKey, TItem[]>()), thisArg);
}

export function toLookupFromList<TItem, TKey>(keyFunc: (i: TItem) => TKey[], thisArg?: any): OperatorFunction<TItem[], Map<TKey, TItem[]>> {
  return map(items => items.reduce((lookup, item) => {
    const keys = keyFunc(item);

    for (const key of keys) {
      let list = lookup.get(key);
      if (!hasValue(list)) {
        list = [];
        lookup.set(key, list);
      }

      list.push(item);
    }

    return lookup;
  }, new Map<TKey, TItem[]>()), thisArg);
}
