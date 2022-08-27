export interface UserInfo {
  Id: string;
  UserName: string;
  AvatarUrl: string;
}

export interface AttentionUpdate {
  UserId: string;
  Score: number;
}