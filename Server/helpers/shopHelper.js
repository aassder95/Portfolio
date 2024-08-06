const fs = require("fs");
const path = require("path");

const outputPath = path.join(__dirname, "../output/shopRandCard.json");

const shopHelper = {
  readShopCards: async () => {
    try {
      if (!fs.existsSync(outputPath)) {
        throw new Error(ERROR_CODES.NOT_FOUND_FILE);
      }

      const data = await fs.promises.readFile(outputPath, "utf8");
      return JSON.parse(data);
    } catch (error) {
      console.error("Error in readShopCards:", error);
      throw error;
    }
  },

  processBuyItem: async (user, card, cardIds, template) => {
    try {
      user.gold -= template.Price;
      card.cnt += template.Count;

      await Promise.all([user.save(), card.save()]);

      return await shopHelper.getCardLists(cardIds);
    } catch (error) {
      console.error("Error in processBuyItem:", error);
      throw error;
    }
  },

  getCardLists: async (purchasedCardIds) => {
    try {
      const allCards = await shopHelper.readShopCards();

      const purchasedCards = allCards.filter((card) =>
        purchasedCardIds.includes(card.Id)
      );

      return {
        allCards: allCards.map((card) => card.Id),
        purchasedCards: purchasedCards.map((card) => card.Id),
      };
    } catch (error) {
      console.error("Error in getCardLists:", error);
      throw error;
    }
  },
};

module.exports = shopHelper;
