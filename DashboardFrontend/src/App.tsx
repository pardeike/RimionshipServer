import './App.css'

import { Component, Show, VoidComponent, createSignal, onCleanup } from 'solid-js'
import { LatestTable } from './LatestTable'
import { useRimionship } from './RimionshipContext'

import { DirectionTable } from './DirectionTable'
import { AttentionTable } from './AttentionTable'
import { EventsTable } from './EventsTable'
import { Link, Route, Routes } from '@solidjs/router'
import { PlayerDetail } from './PlayerDetail'

const DisconnectAlert: VoidComponent<{ disconnectReason: Error }> = (props) => {
  return <div>
    <b>Verbindung zum Server unterbrochen.</b><br />
    <p>{props.disconnectReason?.message}</p>
  </div>
}

const App: Component = () => {
  const { connected, disconnectReason } = useRimionship()

  const [date, setDate] = createSignal(new Date())
  const interval = setInterval(
    () => setDate(() => new Date()),
    1000
  )
  onCleanup(() => clearInterval(interval))

  const gotoAdmin = () => {
    const hostname = window.location.host.replace('3000', '5062')
    window.location.assign(`${window.location.protocol}//${hostname}/admin/`)
    return false
  }

  const currentDate = () => {
    const d = date()
    return `${d.getHours()}:${d.getMinutes()}:${d.getSeconds()}`
  }

  return (
    <div style="padding-left: 5px; width: calc(100% - 10px)">
      <div class="row">
        <div>
          <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light border-bottom mb-3 p-0 position-relative">
            <span style="font-size: 18px; font-weight: 600" class="navbar-brand">Rimionship 2022</span>
            <Link class="navbar-brand" href="#" onClick={() => gotoAdmin()}>Admin</Link>
            <Link class="navbar-brand" href="/">Dashboard</Link>
            <span class="position-absolute end-0" style="font-size: 18px; font-weight: 100; padding-right: 8px">{currentDate()}</span>
          </nav>
        </div>
      </div>

      <Show when={connected()} fallback={<DisconnectAlert disconnectReason={disconnectReason()!} />}>
        <div class="row">
          <div class="col">
            <DirectionTable />
          </div>
          <div class="col">
            <AttentionTable />
          </div>
          <div class="col">
            <EventsTable />
          </div>
        </div>
        <div class="row">
          <div>
            <Routes>
              <Route path="/" component={LatestTable} />
              <Route path="/player/:id" component={PlayerDetail} />
            </Routes>
          </div>
        </div>
      </Show>
    </div>
  )
}

export default App
