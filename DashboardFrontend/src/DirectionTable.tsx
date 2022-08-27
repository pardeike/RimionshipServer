import { VoidComponent } from "solid-js";

export const DirectionTable: VoidComponent = () => {
  return <>
    <h2>Regie-Liste</h2>
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
      </tbody>
    </table>
  </>
};