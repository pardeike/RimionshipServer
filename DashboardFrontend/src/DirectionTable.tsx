import { createEffect, createMemo, For, VoidComponent } from "solid-js";
import { PlayerLink } from "./PlayerLink";
import { useRimionship } from "./RimionshipContext";

const DirectionRow : VoidComponent<{ id: string, comment: string }> = (props) => {
  const { latestStats } = useRimionship();

  const stats = createMemo(() => latestStats.find(p => p.UserId === props.id) );

  return (
  <tr>
    <td>{stats()?.Place}</td>
    <td><PlayerLink id={props.id} /></td>
    <td>{stats()?.Wealth}</td>
    <td>{props.comment}</td>
  </tr>
  );
};

export const DirectionTable: VoidComponent = () => {
  const { directionList } = useRimionship();

  createEffect(() => console.log(Array.from(directionList)));
  return <>
    <h3>Regie-Liste</h3>
    <table class="table">
      <thead>
        <tr class="table-info">
          <th>Platz</th>
          <th>Name</th>
          <th>Koloniewert</th>
          <th>Bemerkung</th>
        </tr>
      </thead>
      <tbody>
        <For each={directionList}>{(row) => <DirectionRow id={row.UserId} comment={row.Comment!}/>}</For>
      </tbody>
    </table>
  </>
};