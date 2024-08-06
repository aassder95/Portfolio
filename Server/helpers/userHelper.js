const User = require("../models/User");
const UserPurchases = require("../models/UserPurchases");
const Stage = require("../models/Stage");
const Card = require("../models/Card");
const Deck = require("../models/Deck");

const userHelper = {
  createDefaultUser: async (uuid, name) => {
    try {
      await User.create({ uuid, name });
      await UserPurchases.create({ user_uuid: uuid, rand_card_ids: [] });
      await Stage.create({ user_uuid: uuid });
      const cards = [];
      for (let i = 0; i < 6; i++) {
        const card = await Card.create({
          user_uuid: uuid,
          template_id: 130000001 + i * 4,
        });
        cards.push(card);
      }
      await Deck.create({
        user_uuid: uuid,
        template_id1: cards[0].template_id,
        template_id2: cards[1].template_id,
        template_id3: cards[2].template_id,
        template_id4: cards[3].template_id,
        template_id5: cards[4].template_id,
      });
    } catch (error) {
      console.error("Error in createDefaultUser:", error);
      throw error;
    }
  },

  updateUserPurchases: async (uuid, cardIds) => {
    try {
      await UserPurchases.update(
        { rand_card_ids: cardIds },
        { where: { user_uuid: uuid } }
      );
    } catch (error) {
      console.error("Error in updateUserPurchases:", error);
      throw error;
    }
  },

  getUser: async (uuid) => {
    try {
      return await User.findOne({ where: { uuid } });
    } catch (error) {
      console.error("Error in getUser:", error);
      throw error;
    }
  },

  getUserInfo: async (uuid) => {
    try {
      return await User.findOne({
        where: { uuid },
        include: [
          { model: Stage, as: "stage" },
          { model: Card, as: "cards" },
          { model: Deck, as: "deck" },
        ],
      });
    } catch (error) {
      console.error("Error in getUserInfo:", error);
      throw error;
    }
  },

  getUserPurchase: async (uuid) => {
    try {
      return await UserPurchases.findOne({ where: { user_uuid: uuid } });
    } catch (error) {
      console.error("Error in getUserPurchase:", error);
      throw error;
    }
  },

  filterUser: (user) => {
    try {
      return {
        name: user.name,
        gold: user.gold,
        gem: user.gem,
        stage: { clearStage: user.stage.single_clear_stage },
        cards: user.cards.map(({ template_id, level, cnt }) => ({
          templateId: template_id,
          level,
          cnt,
        })),
        deck: [
          user.deck.template_id1,
          user.deck.template_id2,
          user.deck.template_id3,
          user.deck.template_id4,
          user.deck.template_id5,
        ],
      };
    } catch (error) {
      console.error("Error in filterUser:", error);
      throw error;
    }
  },
};

module.exports = userHelper;
