import { Accessor, Component, Show, VoidComponent } from 'solid-js';
import { LatestTable } from './LatestTable';
import { useRimionship } from './RimionshipContext';

import './App.css';
import { DirectionTable } from './DirectionTable';
import { AttentionTable } from './AttentionTable';

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
          <div class="col-xl-1"></div>
          <div class="col-xl-5 col-lg-6">
            <DirectionTable />
          </div>
          <div class="col-xl-5 col-lg-6">
            <AttentionTable />
          </div>
        </div>
        <div class="row">
          <div class="col-xl-1"></div>
          <div class="col-xl-10 col-lg-12">
            <LatestTable />
          </div>
        </div>
      </Show>
    </>
  );
};

export default App;
