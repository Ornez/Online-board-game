namespace API.Modules.Game.Keys
{
    public class AllKeys
    {
        public class TokenClaimNames 
        {
            public const string ID = "id";
            public const string USER_NAME = "username";
            public const string ROLE = "userRole";
        }

        public class Roles 
        {
            public const string USER = "User";
            public const string GUEST = "Guest";
        }

        public class LobbyMessages 
        {
            public const string CREATE = "create_lobby";
            public const string UPDATE = "update_lobby";
            public const string DELETE = "delete_lobby";
            public const string PLAYER_CHANGED_READINESS = "player_changed_readiness";
            public const string PLAYER_JOINED = "player_joined";
            public const string PLAYER_LEFT = "player_left";
            public const string GAME_STARTED = "game_started";
            public const string PLAYER_KICKED = "player_kicked";
        }

        public class ChatMessages 
        {
            public const string SEND_MESSAGE = "message_sent";
        }

        public class MapKeys
        {
            public const int MAP_SIZE = 21;
            public const int MAP_CENTER = 10;
            public const string ROUTE_FIELD_PLACED = "route_field_placed";
        }

        public class GameMessages
        {
            public const string OPEN_CHEST_SUCCESS = "OPEN_CHEST_SUCCESS";
            public const string OPEN_CHEST_FAILED = "OPEN_CHEST_FAILED";
        }
        
        public class GameKeys
        {
            public const string PLAYERS_SELECTED = "players_selected";
            public const string ROUND_STARTED = "round_started";
            public const string ROUTE_FIELD_SELECTED = "route_field_selected";
            public const string PLAYER_MOVED = "player_moved";
            public const string ENEMY_SPAWNED = "enemy_spawned";
            public const string FIGHT_STARTED = "fight_started";
            public const string DICE_ROLLED = "dice_rolled";
            public const string FIGHT_OVER = "fight_over";
            public const string FIGHT_CALCULATED = "fight_calculated";
            public const string ROUTE_FIELD_UPDATED = "route_field_updated";
            public const string PLAYER_UPDATED = "player_updated";
            public const string INVENTORY_UPDATED = "inventory_updated";
            
            public const string GAME_ENDED = "game_ended";

            public const string FIGHT_VICTORY = "FIGHT_VICTORY";
            public const string FIGHT_DEFEAT = "FIGHT_DEFEAT";
            public const string FIGHT_DRAW = "FIGHT_DRAW";
        }
    }
}