export interface UserInfo {
  Id: string
  UserName: string
  AvatarUrl: string
}

export interface AttentionUpdate {
  UserId: string
  Score: number
}

export interface DirectionInstruction {
  UserId: string
  Comment?: string
}

export interface EventUpdate {
  UserId: string
  Ticks: number
  Name: string
  Quest: string
  Faction: string
  Points: number
  Strategy: string
  ArrivalMode: string
}