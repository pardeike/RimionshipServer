import { batch, createEffect, onCleanup, onMount, VoidComponent } from "solid-js"
import { produce, reconcile } from 'solid-js/store'
import * as signalR from '@microsoft/signalr'
import * as signalRm from '@microsoft/signalr-protocol-msgpack'
import { useRimionship } from "./RimionshipContext"
import { AttentionUpdate, DirectionInstruction, UserInfo, EventUpdate } from "./MessageDTOs"
import { LatestStats } from "./Stats"

export const CreateSignalRConnection = () => {
  return new signalR.HubConnectionBuilder()
    .withUrl("api/dashboard")
    .withHubProtocol(new signalRm.MessagePackHubProtocol())
    .withAutomaticReconnect({ nextRetryDelayInMilliseconds: (ctx) => Math.min(ctx.previousRetryCount, 5) * 1000 })
    .build()
}

export const SignalRHandler: VoidComponent = () => {
  const {
    connection,
    setLatestStats,
    setUsers,
    connected, setConnected,
    disconnectReason, setDisconnectReason,
    setAttentionList,
    setEventsList,
    setDirectionList
  } = useRimionship()

  let updateTimer: ReturnType<typeof setInterval>

  const POLLING_INTERVAL = 5000

  const updateData = async () => {
    try {
      if (connected()) {
        const data = await connection.invoke<LatestStats[]>('GetLatestStats')
        data.sort((a, b) => a.UserId.localeCompare(b.UserId))

        // This is super funky and I kinda miss Linq.
        const order = data
          .map((v, i) => [v.Wealth, i])
          .sort((a, b) => b[0] - a[0])
          .map((v, i) => [v[1], i])

        for (let p of order)
          data[p[0]].Place = p[1] + 1

        setLatestStats(reconcile(data))

        const attention = await connection.invoke<AttentionUpdate[]>('GetAttentionList')
        attention.sort((a, b) => b.Score - a.Score)
        setAttentionList(reconcile(attention))

        const events = await connection.invoke<EventUpdate[]>('GetEventsList')
        // events.sort((a, b) => a.Ticks - b.Ticks) -- already sorted
        setEventsList(reconcile(events))
      }
    }
    catch (err) {
      console.error('Could not fetch data: ', err)
      // TODO: display this in a more generic error way somewhere, I guess?
    }
    finally {
      setTimeout(updateData, POLLING_INTERVAL)
    }
  }

  onMount(async () => {
    connection.on('AddUser', (user: UserInfo) => {
      setUsers(user.Id, user)
    })

    connection.on('SetDirectionInstruction', (inst: DirectionInstruction) => {
      if (!inst.Comment) {
        setDirectionList(list => list.filter(f => f.UserId !== inst.UserId))
      }
      else {
        setDirectionList(produce(list => {
          const idx = list.findIndex(p => p.UserId === inst.UserId)
          if (idx === -1)
            list.push(inst)
          else
            list[idx].Comment = inst.Comment

          return list
        }))
      }
    })

    while (true) {
      try {
        await connection.start()
        const users = await connection.invoke<UserInfo[]>('GetUsers')
        const directions = await connection.invoke<DirectionInstruction[]>('GetDirectionInstructions')

        batch(() => {
          for (let user of users)
            setUsers(user.Id, user)

          setDirectionList(directions)
          setConnected(true)
          setDisconnectReason(undefined)
        })
        break
      }
      catch (err) {
        setDisconnectReason(err as Error)
        console.error('could not connect to SignalR: ', err)
        await new Promise(f => setTimeout(f, 1000))
      }
    }

    await updateData()
  })

  onCleanup(() => {
    connection.stop()
    clearTimeout(updateTimer)
  })

  createEffect(() => {
    const err = disconnectReason()
    if (!!err && err.message.match(/Status code '40[13]'/)) {
      window.location.href = '/Login?returnUrl=/dashboard.html'
    }
  })

  connection.onclose((err) => batch(() => {
    setConnected(false)
    setDisconnectReason(err)
  }))

  connection.onreconnecting((err) => batch(() => {
    setConnected(false)
    setDisconnectReason(err)
  }))

  connection.onreconnected(() => batch(() => {
    setConnected(true)
    setDisconnectReason(undefined)
  }))

  return <></>
}