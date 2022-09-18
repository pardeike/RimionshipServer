import { useParams } from "@solidjs/router"
import { JSX } from "solid-js"
import { createEffect, createMemo, createSignal, For, Show, VoidComponent } from "solid-js"
import { LatestTable } from "./LatestTable"
import { useRimionship } from "./RimionshipContext"
import { CC, displayValue } from "./Utils"

const MiniColumns = [
  CC('Place', '#'),
  CC('UserId', 'Spieler')
]

const Details = [
  [
    CC('Wealth', 'Koloniereichtum'),
    CC('MapCount', 'Anzahl Karten'),
    CC('Colonists', 'Anzahl Kolonisten'),
    CC('ColonistsNeedTending', 'Verletzte Kolonisten'),
    CC('MedicalConditions', 'Krankeiten'),
    CC('Enemies', 'Feide'),
    CC('WildAnimals', 'Wilde Tiere'),
    CC('TamedAnimals', 'Gezähmte Tiere'),
    CC('Visitors', 'Besucher'),
  ], [
    CC('Prisoners', 'Gefangene'),
    CC('DownedColonists', 'Ohnmächtige Kolonisten'),
    CC('MentalColonists', 'Durchgedrehte Kolonisten'),
    CC('Rooms', 'Bebaute Fläche'),
    CC('Caravans', 'Kolonisten in Karawanen'),
    CC('WeaponDps', 'Waffenkraft'),
    CC('Electricity', 'Verfügbare Energie'),
    CC('Medicine', 'Medizin'),
  ], [
    CC('Food', 'Essen'),
    CC('Fire', 'Brandstellen'),
    CC('Conditions', 'Wetter & Kartenbedingungen'),
    CC('Temperature', 'Temperatur auf Hauptkarte'),
    CC('NumRaidsEnemy', 'Feindliche Überfälle'),
    CC('NumThreatBigs', 'Große Bedrohungen'),
    CC('ColonistsKilled', 'Kolonisten getötet'),
    CC('GreatestPopulation', 'Maximale Anzahl Kolonisten'),
  ], [
    CC('InGameHours', 'Gespielte Stunden'),
    CC('DamageTakenPawns', 'Schaden an Kolonisten'),
    CC('DamageTakenThings', 'Schaden an der Kolonie'),
    CC('DamageDealt', 'Ausgeteilter Schaden'),
    CC('AnimalMeatCreated', 'Geschlachtetes Tierfleisch'),
    CC('AmountBloodCleaned', 'Menge aufgewischtes Blut'),
    CC('TicksLowColonistMood', 'Zeit mit niedriger Moral'),
    CC('TicksIgnoringBloodGod', 'Zeit mit Ignoranz des Blutgottes'),
    CC('Timestamp', 'Letzter Kontakt mit Server'),
  ]
]

const MiniPlayerList: VoidComponent = () => <LatestTable sortable={false} columns={MiniColumns} width={250} />

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
      class="form-control bigger"
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

  const getStoredStat = () => {
    const name = "stat="
    let decodedCookie = decodeURIComponent(document.cookie)
    let ca = decodedCookie.split(';')
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i]
      while (c.charAt(0) == ' ') {
        c = c.substring(1)
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length)
      }
    }
    return "Wealth"
  }

  const user = createMemo(() => users[params.id])
  const stats = createMemo(() => latestStats.find(s => s.UserId === params.id))
  const [selectedStat, setSelectedStat] = createSignal(getStoredStat())

  const statsHeaderColor = (id: string, val: JSX.Element) => {
    if (id == selectedStat())
      return { 'color': 'white', 'background-color': 'black' }
    return { 'color': val == '' ? '#ccc' : '#666' }
  }

  const showGraph = (stat: string) => {
    if (stat == 'Timestamp')
      return () => { }
    else
      return () => {
        setSelectedStat(stat)
        document.cookie = "stat=" + stat
      }
  }

  const currentServerAddress = () => {
    return location.protocol + '//' + location.host.replace("3000", "5062")
  }

  return <div class="row">
    <div class="col" style="flex-grow: 0">
      <MiniPlayerList />
    </div>
    <div class="col" style="flex-grow: 9">
      <ul class="list-group" style="padding-bottom: 20px">
        <li class="list-group-item bg-black"><h1 style="color: white">{user().UserName}</h1></li>
        <li class="list-group-item"><SetDirectionInstruction userId={params.id} /></li>
      </ul>
      <Show when={stats()} fallback={<strong>Keine Daten verfügbar.</strong>}>
        <div class="row align-items-start">
          <For each={Details}>{(detail) =>
            <div class="col">
              <ul class="list-group">
                <For each={detail}>{(col) =>
                  <li class="list-group-item bigger highlight" onClick={showGraph(col.id)}>
                    <span class="bigger" style={statsHeaderColor(col.id, displayValue(stats()![col.id], col.id))}>{col.displayName}</span><br />
                    <b>{displayValue(stats()![col.id], col.id) || '0'}</b>
                  </li>
                }</For>
              </ul>
            </div>
          }</For>
        </div>
      </Show>
    </div>
    <div class="col" style="flex-grow: 0">
      <iframe
        src={`https://player.twitch.tv/?channel=${user().UserName}&parent=${encodeURIComponent(window.location.host)}`}
        width="640"
        height="360"
        class="border border-1 border-primary"
        allowfullscreen>
      </iframe>
      <iframe
        src={`${currentServerAddress()}/api/embedgraph/${selectedStat()}/${user().UserName}/620/340`}
        width="640"
        height="320"
        scrolling="no"
        style={{ 'background-color': 'black' }}>
      </iframe>
    </div>
  </div>
}