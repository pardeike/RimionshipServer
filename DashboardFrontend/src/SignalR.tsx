import { createContext, createSignal, ParentComponent, useContext } from "solid-js";
import { createStore } from "solid-js/store";
import { LatestStats } from "./Stats";
import { CreateSignalRConnection } from "./SignalRHandler";

function create() {
  const [connected, setConnected] = createSignal(false);
  const [disconnectReason, setDisconnectReason] = createSignal<Error | undefined>(new Error('Noch kein Verbindungsaufbau versucht'));
  const [latestStats, setLatestStats] = createStore<LatestStats[]>([]);

  const connection = CreateSignalRConnection();

  return {
    connected, setConnected,
    disconnectReason, setDisconnectReason,
    latestStats, setLatestStats,
    connection
  } as const;
}

export type RimionshipContext = ReturnType<typeof create>;
const context = createContext<RimionshipContext>();

export const RimionshipContextProvider: ParentComponent = (props) => {
  const value = create();


  return <context.Provider value={value}>
    {props.children}
  </context.Provider>
};

export function useRimionship(): RimionshipContext {
  return useContext(context)!;
}