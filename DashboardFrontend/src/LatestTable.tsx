import { formatDistance } from "date-fns";
import { batch, createMemo, createSignal, For, Show, VoidComponent } from "solid-js";
import { useRimionship } from "./SignalR";
import { LatestStats } from "./Stats";
import { de } from "date-fns/locale";

type ColumnId = keyof LatestStats;

const CC = (id: ColumnId, displayName: string, sortable: boolean = true) => {
  return {
    id,
    displayName,
    sortable
  };
}

const Columns = [
  CC('UserName', 'Spieler'),
  CC('AmountBloodCleaned', '🩸🧹'),
  CC('AnimalMeatCreated', '🐑🥩'),
  CC('Caravans', '🚌'),
  CC('Colonists', '👫'),
  CC('ColonistsKilled', '👫✝'),
  CC('ColonistsNeedTending', '👫💉'),
  CC('Conditions', '🤮'),
  CC('DamageDealt', '⚔🔢'),
  CC('DamageTakenPawns', '🤕🔢'),
  CC('Fire', '🔥'),
  CC('InGameHours', '⌚'),
  CC('Timestamp', '🔁')
];

export const LatestTable: VoidComponent = (props) => {
  const { latestStats } = useRimionship();
  const [sortKey, setSortKey] = createSignal<ColumnId | undefined>(undefined);
  const [sortDir, setSortDir] = createSignal<-1 | 1>(1);
  const [stopUpdating, setStopUpdating] = createSignal(false);

  const changeSort = (col: ColumnId) => {
    if (sortKey() === col) {
      if (sortDir() === 1)
        setSortDir(-1);
      else {
        batch(() => {
          setSortKey(undefined);
          setSortDir(1);
        });
      }
    }
    else {
      batch(() => {
        setSortKey(col);
        setSortDir(1);
      })
    }
  };

  const arrowState = (col: ColumnId, dir: -1 | 1) => {
    const sk = sortKey();
    const isActive = (sk === col && sortDir() === dir);
    return {
      "text-secondary": !isActive,
      "text-light": isActive,
    };
  };

  const sort = (a: any, b: any): number => {
    if (a instanceof Array && a[0] instanceof Date && b instanceof Array && b[0] instanceof Date) {
      return a[0].getTime() - b[0].getTime();
    }

    if (typeof a === 'string' && typeof b === 'string') {
      return a.localeCompare(b);
    }

    if (typeof a === 'number' && typeof b === 'number') {
      return a - b;
    }

    console.log('cannot sort', typeof a, typeof b);
    return 0;
  };

  const displayValue = (value: any): string => {
    if (value instanceof Array && value[0] instanceof Date) {
      return formatDistance(value[0], new Date(), { locale: de, includeSeconds: true });
    }

    if (typeof value === 'number') {
      return value.toFixed(0);
    }

    return value;
  }

  const rows = createMemo<LatestStats[]>((prev) => {
    if (stopUpdating())
      return prev ?? latestStats;

    const sk = sortKey();
    if (sk === undefined)
      return latestStats;

    const sd = sortDir();
    return Array.from(latestStats).sort((b, a) => sort(a[sk], b[sk]) * sd);
  });

  return <table class="table table-striped table-hover table-stoppable" classList={{ "table-secondary": stopUpdating() }}>
    <thead>
      <tr class="table-dark sortable">
        <For each={Columns}>{(col) =>
          <th onClick={() => changeSort(col.id)} title={col.id}>
            {col.displayName}
            <Show when={col.sortable}>
              <span classList={arrowState(col.id, 1)}>&#9660;</span><span classList={arrowState(col.id, -1)}>&#9650;</span>
            </Show></th>
        }</For>
      </tr>
    </thead>
    <tbody onMouseEnter={() => setStopUpdating(true)} onMouseLeave={() => setStopUpdating(false)}>
      <For each={rows()}>{(row) =>
        <tr>
          <For each={Columns}>{(col) => <td>{displayValue(row[col.id])}</td>}</For>
        </tr>
      }</For>
    </tbody>
  </table>
};
