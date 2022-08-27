export interface UserInfo {
  Id: string;
  UserName: string;
  AvatarUrl: string;
}

export interface PlayerInfo {
  UserId: string;
  UserName: string;
  Score: number;
  AttentionScore: number;
  Comment: string;
}