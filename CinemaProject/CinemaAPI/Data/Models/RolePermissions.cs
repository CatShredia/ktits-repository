namespace CinemaAPI.Data.Models;

public static class RolePermissions
{
    public static bool CanSendMessage(string roleName, string conversationTypeName)
    {
        return (roleName, conversationTypeName) switch
        {
            (_, "Channel") => roleName is "Owner" or "Moderator", // Member только читает
            _ => true // Owner, Moderator, Member могут писать в Direct, Group, Comments
        };
    }

    public static bool CanDeleteOwnMessage(string roleName, string conversationTypeName)
    {
        return (roleName, conversationTypeName) switch
        {
            ("Member", "Channel") => false, // Member в Channel только читает
            _ => true
        };
    }

    public static bool CanEditOwnMessage(string roleName, string conversationTypeName)
    {
        return (roleName, conversationTypeName) switch
        {
            ("Member", "Channel") => false, // Member в Channel только читает
            _ => true
        };
    }

    public static bool CanDeleteOtherMessage(string roleName)
    {
        return roleName is "Owner" or "Moderator";
    }

    public static bool CanAddParticipant(string roleName)
    {
        return roleName is "Owner" or "Moderator";
    }

    public static bool CanRemoveParticipant(string removerRoleName, string targetRoleName)
    {
        // Moderator не может удалить Owner
        if (targetRoleName == "Owner")
            return false;
        return removerRoleName is "Owner" or "Moderator";
    }

    public static bool CanTransferOwnership(string roleName)
    {
        return roleName == "Owner";
    }

    public static bool CanDeleteConversation(string roleName)
    {
        return roleName == "Owner";
    }

    public static bool ShowParticipants(string conversationTypeName)
    {
        return conversationTypeName != "Direct";
    }
}
