import { VoidComponent } from "solid-js";

export const DirectionTable: VoidComponent = () => {
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
      </tbody>
    </table>
  </>
};