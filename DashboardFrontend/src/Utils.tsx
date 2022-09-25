import { differenceInSeconds, differenceInMinutes, differenceInHours } from "date-fns"
import { JSX } from "solid-js"
import { PlayerLink } from "./PlayerLink"
import { LatestStats } from "./Stats"

export const displayValue = (value: any, col: string): JSX.Element => {
  if (col === 'UserId') {
    return <PlayerLink id={value} />
  }

  if (value instanceof Array && value[0] instanceof Date) {
    var now = new Date()
    var seen = value[0] as Date
    var hours = differenceInHours(now, seen)
    if (hours > 0)
      return hours + 'h'
    var mins = differenceInMinutes(now, seen)
    if (mins > 0)
      return mins + 'm'
    var secs = differenceInSeconds(now, seen)
    return secs + 's'
  }

  if (typeof value === 'number') {
    if (value == 0) return ''
    return value.toLocaleString('de')
  }

  return value
}

// dictionary of ColumnId -> short english name
export const columnNames = {
  'Place': ['#', 'Platz'],
  'UserId': ['Player', 'Spielername'],
  'Wealth': ['Wealth', 'Koloniereichtum'],
  'WasShownTimes': ['Stream', 'Wie oft im Stream gezeigt'],
  'MapCount': ['Map', 'Anzahl Karten'],
  'Colonists': ['Colonist', 'Anzahl Kolonisten'],
  'ColonistsNeedTending': ['Injured', 'Verletzte Kolonisten'],
  'MedicalConditions': ['Med Cond', 'Krankeiten'],
  'Enemies': ['Enemies', 'Feinde'],
  'MentalColonists': ['Mental', 'Durchgedrehte Kolonisten'],
  'DownedColonists': ['Downed', 'Ohnmächtige Kolonisten'],
  'Conditions': ['Map Cond', 'Wetter und Kartenbedingungen'],
  'NumRaidsEnemy': ['Raid', 'Feindliche Überfälle'],
  'NumThreatBigs': ['Thread', 'Große Bedrohungen'],
  'Fire': ['Fire', 'Feuer'],
  'WeaponDps': ['Weapon', 'Waffenkraft'],
  'Electricity': ['Power', 'Verfügbare Energie'],
  'Prisoners': ['Prisoners', 'Gefangene'],
  'Rooms': ['Room', 'Bebaute Fläche'],
  'Medicine': ['Med', 'Medizin'],
  'Food': ['Food', 'Essen'],
  'GreatestPopulation': ['Max Pop', 'Maximale Anzahl Kolonisten'],
  'Caravans': ['Caravan', 'Kolonisten in Karawanen'],
  'WildAnimals': ['Wild Animal', 'Wilde Tiere'],
  'TamedAnimals': ['Tame Animal', 'Gezähmte Tiere'],
  'Visitors': ['Visitors', 'Besucher'],
  'Temperature': ['Temp', 'Temperatur auf Hauptkarte'],
  'InGameHours': ['Hours', 'Gespielte Stunden'],
  'DamageTakenPawns': ['Dam. Pawn', 'Schaden an Kolonisten'],
  'DamageTakenThings': ['Dam. Thing', 'Schaden an der Kolonie'],
  'DamageDealt': ['Dam. Dealt', 'Ausgeteilter Schaden'],
  'TicksIgnoringBloodGod': ['Quest God', 'Zeit mit Ignoranz des Blutgottes'],
  'TicksLowColonistMood': ['Quest Mood', 'Zeit mit niedriger Moral'],
  'ColonistsKilled': ['Casualty', 'Getötete Kolonisten'],
  'AmountBloodCleaned': ['Quest Blood', 'Menge aufgewischtes Blut'],
  'AnimalMeatCreated': ['Quest Meat', 'Geschlachtetes Tierfleisch'],
  'Timestamp': ['Seen Ago', 'Zuletzt Daten Gesendet'],
}

export type ColumnId = keyof LatestStats

export const CC = (id: ColumnId, sortable: boolean = true) => {
  return {
    id,
    shortName: columnNames[id][0],
    displayName: columnNames[id][1],
    sortable
  }
}

export type ColumnDef = ReturnType<typeof CC>