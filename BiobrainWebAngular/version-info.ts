

export const versionInfo = (() => {
    try {
      // tslint:disable-next-line:no-var-requires
      return { tag: 'v0.0.0', hash: 'dev' };
    } catch {
      // In dev the file might not exist:
      return { tag: 'v0.0.0', hash: 'dev' };
    }
  })();