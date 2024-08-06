const ERROR_CODES = require("../constants/errorCodes");
const userHelper = require("../helpers/userHelper");
const stageHelper = require("../helpers/stageHelper");
const templateUtils = require("../utils/templates");

const stageService = {
  stageEnd: async (uuid, data) => {
    try {
      if (data.type === 1 && !data.isWin) return;

      const user = await userHelper.getUser(uuid);
      if (!user) return { errorCode: ERROR_CODES.NOT_FOUND_USER };

      const stage = await stageHelper.getStage(uuid);
      if (!stage) return { errorCode: ERROR_CODES.NOT_FOUND_STAGE };

      const template = await templateUtils.getStageTemplate(
        data.type,
        data.type === 2 ? 0 : data.clearStage
      );
      if (!template) return { errorCode: ERROR_CODES.NOT_FOUND_TEMPLATE };

      await stageHelper.processWin(user, stage, data, template);

      return {
        data: {
          gold: user.gold,
          ...(data.type === 1 && { clearStage: stage.single_clear_stage }),
        },
      };
    } catch (error) {
      console.error("Error in stageEnd:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },

  rank: async (uuid, data) => {
    try {
      const stage = await stageHelper.getStage(uuid);
      if (!stage) return { errorCode: ERROR_CODES.NOT_FOUND_STAGE };

      const [rank, myRank] = await Promise.all([
        stageHelper.getRank(data),
        stageHelper.getMyRank(stage, data),
      ]);

      return { data: { rank, myRank } };
    } catch (error) {
      console.error("Error in rank:", error);
      return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
    }
  },
};

module.exports = stageService;
