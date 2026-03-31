import { ICellRendererParams } from 'ag-grid-community';

import { getScoreColor } from './get-score-color';

export function getMaybeNotApplicableCellColor(params: ICellRendererParams): { 'color': string } | undefined {
  const value = params.value as string;
  if (value === 'NA') {
    return { color: 'red' };
  }
  if (value === 'A') {
    return { color: 'red' };
  }

  const progress = Number.parseFloat(value.replace('%', ''));
  if (Number.isNaN(progress)) return undefined;

  return {
    color: getScoreColor(progress)
  };
}
