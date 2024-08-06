const ERROR_CODES = require("../constants/errorCodes");
const userHelper = require("../helpers/userHelper");
const cardHelper = require("../helpers/cardHelper");
const templateUtils = require("../utils/templates");

const cardService = {
  cardLevelUp: async (uuid, data) => {
    try {
      const user = await userHelper.getUser(uuid);
      if (!user) return { errorCode: ERROR_CODES.NOT_FOUND_USER };

      const card = await cardHelper.getCard(uuid, data.id);
      if (!card) return { errorCode: ERROR_CODES.NOT_FOUND_CARD };

      const nextLevel = card.level + 1;
      const nextFishLevelTemplate =
        templateUtils.getFishLevelTemplate(nextLevel);

      if (!nextFishLevelTemplate)
        return { errorCode: ERROR_CODES.MAX_CARD_LEVEL };
      if (user.gold < nextFishLevelTemplate.Gold)
        return { errorCode: ERROR_CODES.NOT_ENOUGH_GOLD };
      if (card.cnt < nextFishLevelTemplate.CardCount)
        return { errorCode: ERROR_CODES.NOT_ENOUGH_CARD };

      await cardHelper.processLevelUp(user, card, nextFishLevelTemplate);

      return {
        data: {
          card: {
            templateId: card.template_id,
            level: card.level,
            cnt: card.cnt,
          },
          gold: user.gold,
        },
      };
    } catch (error) {
      console.error("Error in cardLevelUp:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },

  deckChange: async (uuid, data) => {
    try {
      const deck = await cardHelper.getDeck(uuid);
      if (!deck) return { errorCode: ERROR_CODES.NOT_FOUND_DECK };

      const changeDeck = await cardHelper.changeDeck(
        deck,
        data.deckId,
        data.selectCardId
      );

      return { data: { deck: changeDeck } };
    } catch (error) {
      console.error("Error in deckChange:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },
};

module.exports = cardService;
