
export function removeTags(text: string): string {
    let htmlTagRegEx = /<[^>]*>/g;
    return text ? text.replace(htmlTagRegEx, ''): text;
  }