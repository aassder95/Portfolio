const { Sequelize } = require("sequelize");
const User = require("../models/User");
const Stage = require("../models/Stage");

const stageHelper = {
  processWin: async (user, stage, data, template) => {
    try {
      if (data.type === 1) {
        await stage.update({
          single_clear_stage: data.clearStage,
          single_clear_date: new Date(),
        });

        user.gold += template.Gold;
      } else if (data.type === 2) {
        if (data.clearStage > stage.infinite_clear_wave) {
          await stage.update({
            infinite_clear_wave: data.clearStage - 1,
            infinite_clear_date: new Date(),
          });
        }

        user.gold += template.Gold * (data.clearStage - 1);
      }

      await user.save();
    } catch (error) {
      console.error("Error in processWin:", error);
      throw error;
    }
  },

  getStage: async (uuid) => {
    try {
      return await Stage.findOne({ where: { user_uuid: uuid } });
    } catch (error) {
      console.error("Error in getStage:", error);
      throw error;
    }
  },

  getRank: async (data) => {
    try {
      const condition =
        data.type === 1
          ? { single_clear_stage: { [Sequelize.Op.gt]: 0 } }
          : { infinite_clear_wave: { [Sequelize.Op.gt]: 0 } };
      const orderField =
        data.type === 1 ? "single_clear_stage" : "infinite_clear_wave";
      const dateField =
        data.type === 1 ? "single_clear_date" : "infinite_clear_date";

      const topStages = await Stage.findAll({
        where: condition,
        include: [{ model: User, attributes: ["name"], as: "user" }],
        order: [
          [orderField, "DESC"],
          [dateField, "ASC"],
        ],
        limit: 100,
      });

      return topStages.map((stage, index) => ({
        rank: index + 1,
        name: stage.user.name,
        clearStage: stage[orderField],
      }));
    } catch (error) {
      console.error("Error in getRank:", error);
      throw error;
    }
  },

  getMyRank: async (stage, data) => {
    try {
      const stageField =
        data.type === 1 ? "single_clear_stage" : "infinite_clear_wave";
      const dateField =
        data.type === 1 ? "single_clear_date" : "infinite_clear_date";
      const clearValue = stage[stageField];
      const clearDate = stage[dateField];

      if (clearValue === 0) {
        return {
          rank: 0,
          clearStage: 0,
        };
      }

      const rank = await Stage.count({
        where: {
          [Sequelize.Op.or]: [
            { [stageField]: { [Sequelize.Op.gt]: clearValue } },
            {
              [stageField]: clearValue,
              [dateField]: { [Sequelize.Op.lt]: clearDate },
            },
          ],
        },
      });

      return {
        rank: rank + 1,
        clearStage: clearValue,
      };
    } catch (error) {
      console.error("Error in getMyRank:", error);
      throw error;
    }
  },
};

module.exports = stageHelper;
