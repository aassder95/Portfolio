const { Sequelize } = require("sequelize");
const Chat = require("../models/Chat");

const chatHelper = {
  createChat: async (uuid, user, message) => {
    try {
      return await Chat.create({
        user_uuid: uuid,
        user_name: user.name,
        message,
      });
    } catch (error) {
      console.error("Error in createChat:", error);
      throw error;
    }
  },

  getChat: async (user) => {
    try {
      const chats = await Chat.findAll({
        where: { sent_at: { [Sequelize.Op.gt]: user.last_login } },
        attributes: ["idx", "user_name", "message"],
        order: [["sent_at", "ASC"]],
      });

      return chats.map(({ idx, user_name, message }) => ({
        idx,
        name: user_name,
        message,
      }));
    } catch (error) {
      console.error("Error in getChat:", error);
      throw error;
    }
  },
};

module.exports = chatHelper;
