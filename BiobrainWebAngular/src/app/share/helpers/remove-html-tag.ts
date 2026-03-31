
export function removeHtmlTag(text: string): string {
    return text ? text.replace('<html>','').replace('</html>',''): text;
  }
  