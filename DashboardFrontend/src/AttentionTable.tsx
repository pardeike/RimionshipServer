import { createMemo, For, VoidComponent } from "solid-js";
import { PlayerLink } from "./PlayerLink";
import { useRimionship } from "./RimionshipContext";

const AttentionRow : VoidComponent<{ id: string, score: number }> = (props) => {
  const { latestStats } = useRimionship();

  const stats = createMemo(() => latestStats.find(p => p.UserId === props.id) );

  return (
  <tr>
    <td>{stats()?.Place}</td>
    <td><PlayerLink id={props.id} /></td>
    <td>{stats()?.Wealth}</td>
    <td>{props.score}</td>
  </tr>
  );
};

export const AttentionTable: VoidComponent = () => {
  const { attentionList } = useRimionship();
  return <>
    <h3>Interesting Liste</h3>
    <table class="table">
      <thead>
        <tr class="table-warning">
          <th>Platz</th>
          <th>Name</th>
          <th>Koloniewert</th>
          <th>Score</th>
        </tr>
      </thead>
      <tbody>
        <For each={attentionList}>{(row, idx) =>
          <AttentionRow id={row.UserId} score={row.Score} />
        }</For>
      </tbody>
    </table>
  </>
};