import {UserRole} from "@auth/enums/user-role.enum";

export interface TokenData {
  id: string;
  username: string;
  userRole: UserRole;
}
