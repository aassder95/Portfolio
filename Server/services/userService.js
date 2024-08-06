const ERROR_CODES = require("../constants/errorCodes");
const userHelper = require("../helpers/userHelper");
const cardHelper = require("../helpers/cardHelper");
const shopHelper = require("../helpers/shopHelper");

const userService = {
  login: async (uuid) => {
    try {
      const user = await userHelper.getUserInfo(uuid);
      if (!user) return { errorCode: ERROR_CODES.CREATE_USER };

      await user.update({ last_login: new Date() });

      const userPurchase = await userHelper.getUserPurchase(uuid);
      if (!userPurchase)
        return { errorCode: ERROR_CODES.NOT_FOUND_USER_PURCHASES };

      return {
        data: {
          ...userHelper.filterUser(user),
          ...(await shopHelper.getCardLists(userPurchase.rand_card_ids)),
        },
      };
    } catch (error) {
      console.error("Error in login:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },

  createUser: async (uuid, data) => {
    try {
      await userHelper.createDefaultUser(uuid, data.name);

      const user = await userHelper.getUserInfo(uuid);
      if (!user) return { errorCode: ERROR_CODES.NOT_FOUND_USER };

      const userPurchase = await userHelper.getUserPurchase(uuid);
      if (!userPurchase)
        return { errorCode: ERROR_CODES.NOT_FOUND_USER_PURCHASES };

      return {
        data: {
          ...userHelper.filterUser(user),
          ...(await shopHelper.getCardLists(userPurchase.rand_card_ids)),
        },
      };
    } catch (error) {
      console.error("Error in createUser:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },
};

module.exports = userService;
