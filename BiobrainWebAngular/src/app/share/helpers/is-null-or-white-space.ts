export function isNullOrWhitespace(input: string | undefined | null): input is null | undefined {
  if (input === undefined || input === null) {
    return true;
  }

  return input.replace(/\s/g, '').length < 1;
}
