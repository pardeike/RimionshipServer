import { formatDistance } from "date-fns";
import { de } from "date-fns/locale";
import { Accessor, createRenderEffect, JSX, Signal } from "solid-js";
import { PlayerLink } from "./PlayerLink";
import { LatestStats } from "./Stats";

export const displayValue = (value: any, col: string): JSX.Element => {
  if (col === 'UserId') {
    return <PlayerLink id={value} />
  }

  if (value instanceof Array && value[0] instanceof Date) {
    return formatDistance(value[0], new Date(), { locale: de, includeSeconds: true });
  }

  if (typeof value === 'number') {
    return value.toFixed(0);
  }

  return value;
}

export type ColumnId = keyof LatestStats;

export const CC = (id: ColumnId, displayName: string, sortable: boolean = true) => {
  return {
    id,
    displayName,
    sortable
  };
}

export type ColumnDef = ReturnType<typeof CC>;