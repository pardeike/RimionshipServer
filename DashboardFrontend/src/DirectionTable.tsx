import { createEffect, createMemo, For, VoidComponent } from "solid-js"
import { PlayerLink } from "./PlayerLink"
import { useRimionship } from "./RimionshipContext"

const DirectionRow: VoidComponent<{ id: string, comment: string }> = (props) => {
  const { connection, latestStats } = useRimionship()

  const stats = createMemo(() => latestStats.find(p => p.UserId === props.id))

  const save = async () => {
    try {
      await connection.invoke('SetDirectionInstruction', props.id, '')
    }
    catch (err) {
      console.error('Could not clear instruction:', err)
    }
    finally {
    }
  }

  return (
    <tr>
      <td>{stats()?.Place}</td>
      <td><PlayerLink id={props.id} /></td>
      <td>{stats()?.Wealth}</td>
      <td>{props.comment} <span style="float: right; cursor: not-allowed" onClick={save}>❌</span></td>
    </tr>
  )
}

export const DirectionTable: VoidComponent = () => {
  const { directionList } = useRimionship()
  return <>
    <h3>Regie</h3>
    <table class="table">
      <thead>
        <tr class="table-info">
          <th>Platz</th>
          <th>Spieler</th>
          <th>Koloniewert</th>
          <th>Bemerkung</th>
        </tr>
      </thead>
      <tbody>
        <For each={directionList}>{(row) => <DirectionRow id={row.UserId} comment={row.Comment!} />}</For>
      </tbody>
    </table>
  </>
}