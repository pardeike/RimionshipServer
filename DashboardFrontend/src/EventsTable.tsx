import { createMemo, For, VoidComponent } from "solid-js"
import { EventUpdate } from "./MessageDTOs"
import { PlayerLink } from "./PlayerLink"
import { useRimionship } from "./RimionshipContext"

const EventRow: VoidComponent<{ evt: EventUpdate }> = (props) => {

  function ticksToText(ticks: number) {
    const hours = Math.floor(ticks / 250) / 10
    return `${hours} hours`
  }

  return (
    <tr>
      <td><PlayerLink id={props.evt.UserId} /></td>
      <td>{ticksToText(props.evt.Ticks)}</td>
      <td>{props.evt.Name}</td>
      <td>{props.evt.Quest}</td>
      <td>{props.evt.Faction}</td>
      <td>{Math.floor(props.evt.Points)}</td>
      <td>{props.evt.Strategy}</td>
      <td>{props.evt.ArrivalMode}</td>
    </tr>
  )
}

export const EventsTable: VoidComponent = () => {
  const { eventsList } = useRimionship()
  return <>
    <h3>Zukünftige Ereignisse</h3>
    <table class="table">
      <thead>
        <tr class="table-warning">
          <th>Spieler</th>
          <th>Zeitpunkt</th>
          <th>Typ</th>
          <th>Auftrag</th>
          <th>Faktion</th>
          <th>Punkte</th>
          <th>Strategie</th>
          <th>Modus</th>
        </tr>
      </thead>
      <tbody>
        <For each={eventsList}>{(row, idx) =>
          <EventRow evt={row} />
        }</For>
      </tbody>
    </table>
  </>
}