import { useParams } from "@solidjs/router"
import { createEffect, createMemo, createRenderEffect, createSignal, For, Show, VoidComponent } from "solid-js"
import { LatestTable } from "./LatestTable"
import { useRimionship } from "./RimionshipContext"
import { CC, displayValue } from "./Utils"

const MiniColumns = [
  CC('Place', '#'),
  CC('UserId', 'Spieler')
]

const DetailColumns = [
  CC('Wealth', 'Koloniewert'),
  CC('AmountBloodCleaned', 'Blutsäuberung'),
  CC('AnimalMeatCreated', 'Tierfleischerzeugnisse'),
  CC('Caravans', 'Karnevalle'),
  CC('Colonists', 'Darmisten'),
  CC('Fire', 'Feuer!'),
  CC('Electricity', 'Stroom'),
  CC('Timestamp', 'Letztes Update'),
  CC('MedicalConditions', 'Medizinische Bedingungen')
]

const MiniPlayerList: VoidComponent = () => <LatestTable sortable={false} columns={MiniColumns} />

const SetDirectionInstruction: VoidComponent<{ userId: string }> = (props) => {
  const { connection, directionList } = useRimionship()

  const [saving, setSaving] = createSignal(false)
  const [disabled, setDisabled] = createSignal(true)
  const [comment, setComment] = createSignal('')

  createEffect((prevUser) => {
    const dir = directionList.find(f => f.UserId === props.userId)?.Comment
    const val = setComment(prev => dir ?? (prevUser === props.userId) ? prev : '')
    inputEle.value = dir ?? ''
    return prevUser
  })

  let inputEle: HTMLInputElement = null!

  const setText = (text: string) => {
    setComment(text)
    setDisabled(false)
  }

  const save = async () => {
    setDisabled(true)
    setSaving(true)
    try {
      await connection.invoke('SetDirectionInstruction', props.userId, comment())
    }
    catch (err) {
      setDisabled(false)
      console.error('Could not save instruction:', err)
    }
    finally {
      setSaving(false)
    }
  }
  return <div class="input-group">
    <input
      type="text"
      class="form-control"
      value={comment()}
      placeholder="Regie-Kommentar"
      onInput={e => setText((e.currentTarget as HTMLInputElement).value)}
      onKeyDown={e => e.key === 'Enter' && save()}
      disabled={saving()}
      ref={inputEle} />
    <button class="btn btn-secondary" type="button" disabled={disabled()} onClick={save}>Speichern</button>
  </div>
}

export const PlayerDetail: VoidComponent = () => {
  const params = useParams<{ id: string }>()
  const { latestStats, users } = useRimionship()

  const user = createMemo(() => users[params.id])
  const stats = createMemo(() => latestStats.find(s => s.UserId === params.id))

  return <div class="row">
    <div class="col-2">
      <MiniPlayerList />
    </div>
    <div class="col-5">
      <h1>{user().UserName}</h1>
      <SetDirectionInstruction userId={params.id} />
      <Show when={stats()} fallback={<strong>Keine Daten verfügbar.</strong>}>
        <div class="row align-items-start">
          <For each={DetailColumns}>{(col) =>
            <div class="col-3">
              <dt>{col.displayName}</dt>
              <dd>{displayValue(stats()![col.id], col.id)}</dd>
            </div>
          }</For>
        </div>
      </Show>
    </div>
    <div class="col">
      <iframe
        src={`https://player.twitch.tv/?channel=${user().UserName}&parent=${encodeURIComponent(window.location.host)}`}
        width="640"
        height="480"
        class="border border-1 border-primary"
        allowfullscreen>
      </iframe>
    </div>
  </div>
}