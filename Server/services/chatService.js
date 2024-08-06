const ERROR_CODES = require("../constants/errorCodes");
const userHelper = require("../helpers/userHelper");
const chatHelper = require("../helpers/chatHelper");

const chatService = {
  getUserAndChats: async (uuid) => {
    const user = await userHelper.getUser(uuid);
    if (!user) return { errorCode: ERROR_CODES.NOT_FOUND_USER };

    const chats = await chatHelper.getChat(user);
    return { user, chats };
  },

  chat: async (uuid) => {
    try {
      const result = await chatService.getUserAndChats(uuid);
      if (result.errorCode) return result;

      return { data: { chats: result.chats } };
    } catch (error) {
      console.error("Error in chat:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },

  chatSend: async (uuid, data) => {
    try {
      const result = await chatService.getUserAndChats(uuid);
      if (result.errorCode) return result;

      await chatHelper.createChat(uuid, result.user, data.message);
      const chats = await chatHelper.getChat(result.user);

      return { data: { chats } };
    } catch (error) {
      console.error("Error in chatSend:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },
};

module.exports = chatService;
