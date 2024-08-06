const ERROR_CODES = require("../constants/errorCodes");
const shopHelper = require("../helpers/shopHelper");
const userHelper = require("../helpers/userHelper");
const cardHelper = require("../helpers/cardHelper");
const templateUtils = require("../utils/templates");

const shopService = {
  shopRandCard: async (uuid) => {
    try {
      const userPurchase = await userHelper.getUserPurchase(uuid);
      if (!userPurchase)
        return { errorCode: ERROR_CODES.NOT_FOUND_USER_PURCHASES };

      const data = await shopHelper.getCardLists(userPurchase.rand_card_ids);
      return { data };
    } catch (error) {
      console.error("Error in shopRandCard:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },

  shopBuyItem: async (uuid, data) => {
    try {
      const template = await templateUtils.getShopTemplate(data.id);
      if (!template) return { errorCode: ERROR_CODES.NOT_FOUND_TEMPLATE };

      const user = await userHelper.getUser(uuid);
      if (!user) return { errorCode: ERROR_CODES.NOT_FOUND_USER };
      if (user.gold < template.Price)
        return { errorCode: ERROR_CODES.NOT_ENOUGH_GOLD };

      const userPurchase = await userHelper.getUserPurchase(uuid);
      if (!userPurchase)
        return { errorCode: ERROR_CODES.NOT_FOUND_USER_PURCHASES };

      if (userPurchase.rand_card_ids.includes(data.id)) {
        return { errorCode: ERROR_CODES.ALREADY_PURCHASED };
      }

      let card = await cardHelper.getCard(uuid, template.TemplateId);
      if (!card) card = await cardHelper.createCard(uuid, template.TemplateId);

      userPurchase.rand_card_ids.push(data.id);
      await userHelper.updateUserPurchases(
        user.uuid,
        userPurchase.rand_card_ids
      );

      const { allCards, purchasedCards } = await shopHelper.processBuyItem(
        user,
        card,
        userPurchase.rand_card_ids,
        template
      );

      return {
        data: {
          card: {
            templateId: card.template_id,
            level: card.level,
            cnt: card.cnt,
          },
          gold: user.gold,
          allCards,
          purchasedCards,
        },
      };
    } catch (error) {
      console.error("Error in shopBuyItem:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },
};

module.exports = shopService;
