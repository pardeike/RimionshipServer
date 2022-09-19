import { createContext, createSignal, ParentComponent, useContext } from "solid-js"
import { createStore } from "solid-js/store"
import { LatestStats } from "./Stats"
import { CreateSignalRConnection } from "./SignalRHandler"
import { AttentionUpdate, DirectionInstruction, UserInfo, EventUpdate } from "./MessageDTOs"

function create() {
  const [connected, setConnected] = createSignal(false)
  const [disconnectReason, setDisconnectReason] = createSignal<Error | undefined>(new Error('Noch kein Verbindungsaufbau versucht'))
  const [users, setUsers] = createStore<{ [userId: string]: UserInfo }>({})
  const [latestStats, setLatestStats] = createStore<LatestStats[]>([])
  const [attentionList, setAttentionList] = createStore<AttentionUpdate[]>([])
  const [eventsList, setEventsList] = createStore<EventUpdate[]>([])
  const [directionList, setDirectionList] = createStore<DirectionInstruction[]>([])

  const connection = CreateSignalRConnection()

  const switchTwitchChannel = async (userId: string) => {
    await connection.invoke<void>('SwitchTwitchChannel', userId)
  }

  const setAttentionReduction = async (delta: number): Promise<number> => {
    return await connection.invoke<number>('SetAttentionReduction', delta)
  }

  return {
    connected, setConnected,
    disconnectReason, setDisconnectReason,
    users, setUsers,
    latestStats, setLatestStats,
    attentionList, setAttentionList,
    eventsList, setEventsList,
    directionList, setDirectionList,
    connection,
    switchTwitchChannel,
    setAttentionReduction
  } as const
}

export type RimionshipContext = ReturnType<typeof create>
const context = createContext<RimionshipContext>()

export const RimionshipContextProvider: ParentComponent = (props) => {
  const value = create()


  return <context.Provider value={value}>
    {props.children}
  </context.Provider>
}

export function useRimionship(): RimionshipContext {
  return useContext(context)!
}