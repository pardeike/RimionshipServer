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
    <div class="toptable">
      <table class="table">
        <thead>
          <tr class="table-warning">
            <th class="sticky">Spieler</th>
            <th class="sticky">Zeitpunkt</th>
            <th class="sticky">Typ</th>
            <th class="sticky">Auftrag</th>
            <th class="sticky">Faktion</th>
            <th class="sticky">Punkte</th>
            <th class="sticky">Strategie</th>
            <th class="sticky">Modus</th>
          </tr>
        </thead>
        <tbody>
          <For each={eventsList}>{(row, idx) =>
            <EventRow evt={row} />
          }</For>
        </tbody>
      </table>
    </div>
  </>
}