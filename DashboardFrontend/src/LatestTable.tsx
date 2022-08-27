import { batch, createMemo, createSignal, For, mergeProps, Show, VoidComponent } from "solid-js";
import { useRimionship } from "./RimionshipContext";
import { LatestStats } from "./Stats";
import { CC, ColumnDef, ColumnId, displayValue } from "./Utils";

const DefaultColumns = [
  CC('Place', '#'),
  CC('UserId', 'Spieler'),
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
  CC('Wealth', '💲'),
  CC('Timestamp', '🔁')
];

export const LatestTable: VoidComponent<{ sortable: boolean, columns: ColumnDef[] }> = (props) => {
  props = mergeProps({ sortable: true, columns: DefaultColumns }, props);
  const { latestStats } = useRimionship();
  const [sortKey, setSortKey] = createSignal<ColumnId | undefined>('Place');
  const [sortDir, setSortDir] = createSignal<-1 | 1>(-1);
  const [stopUpdating, setStopUpdating] = createSignal(false);

  const changeSort = (col: ColumnId) => {
    if (!props.sortable)
      return;

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
        <For each={props.columns}>{(col) =>
          <th onClick={() => changeSort(col.id)} title={col.id}>
            {col.displayName}
            <Show when={col.sortable && props.sortable}>
              <span classList={arrowState(col.id, 1)}>&#9660;</span><span classList={arrowState(col.id, -1)}>&#9650;</span>
            </Show></th>
        }</For>
      </tr>
    </thead>
    <tbody onMouseEnter={() => setStopUpdating(true)} onMouseLeave={() => setStopUpdating(false)}>
      <For each={rows()}>{(row) =>
        <tr>
          <For each={props.columns}>{(col) => <td>{displayValue(row[col.id], col.id)}</td>}</For>
        </tr>
      }</For>
    </tbody>
  </table>
};
