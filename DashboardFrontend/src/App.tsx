import { Accessor, Component, Show, VoidComponent } from 'solid-js';
import { LatestTable } from './LatestTable';
import { useRimionship } from './RimionshipContext';

import './App.css';

interface Player {
  name: string;
  score: number;
  hint?: string;
  values: number[];
}

const DisconnectAlert: VoidComponent<{ disconnectReason: Error }> = (props) => {
  return <div class="alert alert-danger">
    <h4>Verbindung zum Server unterbrochen.</h4>
    <p>{props.disconnectReason?.message}</p>
  </div>
};

const App: Component = () => {
  const { connected, disconnectReason } = useRimionship();

  return (
    <>
      <Show when={connected()} fallback={<DisconnectAlert disconnectReason={disconnectReason()!} />}>
        <div class="row">
          <div class="col-10">
            <LatestTable />
          </div>
        </div>
      </Show>
    </>
  );
};

export default App;
