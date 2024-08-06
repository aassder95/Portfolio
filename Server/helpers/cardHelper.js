const Card = require("../models/Card");
const Deck = require("../models/Deck");

const cardHelper = {
  createCard: async (uuid, id) => {
    try {
      return await Card.create({ user_uuid: uuid, template_id: id });
    } catch (error) {
      console.error("Error in createCard:", error);
      throw error;
    }
  },

  processLevelUp: async (user, card, nextFishLevelTemplate) => {
    try {
      user.gold -= nextFishLevelTemplate.Gold;
      card.level = nextFishLevelTemplate.Level;
      card.cnt -= nextFishLevelTemplate.CardCount;

      await Promise.all([user.save(), card.save()]);
    } catch (error) {
      console.error("Error in processLevelUp:", error);
      throw error;
    }
  },

  getCard: async (uuid, id) => {
    try {
      return await Card.findOne({
        where: { user_uuid: uuid, template_id: id },
      });
    } catch (error) {
      console.error("Error in getCard:", error);
      throw error;
    }
  },

  getCards: async (uuid) => {
    try {
      return await Card.findAll({ where: { user_uuid: uuid } });
    } catch (error) {
      console.error("Error in getCards:", error);
      throw error;
    }
  },

  getDeck: async (uuid) => {
    try {
      return await Deck.findOne({ where: { user_uuid: uuid } });
    } catch (error) {
      console.error("Error in getDeck:", error);
      throw error;
    }
  },

  changeDeck: async (deck, deckId, selectId) => {
    try {
      const deckFields = [
        "template_id1",
        "template_id2",
        "template_id3",
        "template_id4",
        "template_id5",
      ];

      const updatedDeck = deckFields.map((field) =>
        deck[field] === deckId ? selectId : deck[field]
      );
      deckFields.forEach((field, index) => (deck[field] = updatedDeck[index]));

      await deck.save();

      return updatedDeck;
    } catch (error) {
      console.error("Error in changeDeck:", error);
      throw error;
    }
  },
};

module.exports = cardHelper;
