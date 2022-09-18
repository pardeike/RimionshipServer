import './App.css'

import { Component, Show, VoidComponent } from 'solid-js'
import { LatestTable } from './LatestTable'
import { useRimionship } from './RimionshipContext'

import { DirectionTable } from './DirectionTable'
import { AttentionTable } from './AttentionTable'
import { EventsTable } from './EventsTable'
import { Link, Route, Routes } from '@solidjs/router'
import { PlayerDetail } from './PlayerDetail'

const DisconnectAlert: VoidComponent<{ disconnectReason: Error }> = (props) => {
  return <div class="alert alert-danger">
    <h4>Verbindung zum Server unterbrochen.</h4>
    <p>{props.disconnectReason?.message}</p>
  </div>
}

const App: Component = () => {
  const { connected, disconnectReason } = useRimionship()

  return (
    <>
      <div class="row">
        <div>
          <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <Link class="navbar-brand" href="/">Rimionship Dasboard</Link>
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
    </>
  )
}

export default App
