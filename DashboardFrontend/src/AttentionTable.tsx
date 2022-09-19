import { createMemo, For, VoidComponent, createSignal } from "solid-js"
import { PlayerLink } from "./PlayerLink"
import { useRimionship } from "./RimionshipContext"

const AttentionRow: VoidComponent<{ id: string, score: number }> = (props) => {
  const { latestStats } = useRimionship()

  const stats = createMemo(() => latestStats.find(p => p.UserId === props.id))

  return (
    <tr>
      <td>{stats()?.Place}</td>
      <td><PlayerLink id={props.id} /></td>
      <td>{stats()?.Wealth}</td>
      <td>{props.score}</td>
    </tr>
  )
}

export const AttentionTable: VoidComponent = () => {
  const { attentionList, setAttentionReduction } = useRimionship()

  const [delta, setDelta] = createSignal(0)

  const updateDelta = async (delta: number) => {
    setDelta(await setAttentionReduction(delta))
  }
  updateDelta(0)

  return <>
    <h3>Interessante Spieler</h3>
    <table class="table">
      <thead>
        <tr class="table-warning">
          <th>Platz</th>
          <th>Spieler</th>
          <th>Koloniewert</th>
          <th>
            <div class="row">
              <div class="col">
                Punkte
              </div>
              <div class="col end stepper" style="text-align: right; white-space: nowrap">
                Abzug: {delta} Punkt{delta() == 1 ? '' : 'e'}/sek &nbsp;
                <button onClick={() => updateDelta(-1)}><span>–</span></button> &nbsp;
                <button onClick={() => updateDelta(1)}><span>+</span></button>
              </div>
            </div>
          </th>
        </tr>
      </thead>
      <tbody>
        <For each={attentionList}>{(row, idx) =>
          <AttentionRow id={row.UserId} score={row.Score} />
        }</For>
      </tbody>
    </table>
  </>
}