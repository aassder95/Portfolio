const express = require("express");
const PROTOCOLS = require("../constants/protocols");
const ERROR_CODES = require("../constants/errorCodes");
const userService = require("../services/userService");
const stageService = require("../services/stageService");
const cardService = require("../services/cardService");
const chatService = require("../services/chatService");
const shopService = require("../services/shopService");

const protocolRoutes = express.Router();

const services = {
  [PROTOCOLS.LOGIN]: userService.login,
  [PROTOCOLS.CREATE_USER]: userService.createUser,
  [PROTOCOLS.CARD_LEVEL_UP]: cardService.cardLevelUp,
  [PROTOCOLS.STAGE_END]: stageService.stageEnd,
  [PROTOCOLS.RANK]: stageService.rank,
  [PROTOCOLS.CHAT]: chatService.chat,
  [PROTOCOLS.CHAT_SEND]: chatService.chatSend,
  [PROTOCOLS.DECK_CHANGE]: cardService.deckChange,
  [PROTOCOLS.SHOP_RAND_CARD]: shopService.shopRandCard,
  [PROTOCOLS.SHOP_BUY_ITEM]: shopService.shopBuyItem,
};

function errorHandler(err, req, res, next) {
  console.error(err.stack);
  res.status(ERROR_CODES.INTERNAL_SERVER_ERROR).json({
    errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR,
  });
}

async function handleProtocol(protocol, uuid, data) {
  if (!services[protocol]) {
    return { errorCode: ERROR_CODES.BAD_REQUEST };
  }

  try {
    const result = await services[protocol](uuid, data);
    return { errorCode: ERROR_CODES.OK, ...result };
  } catch (error) {
    console.error(
      `Error handling protocol ${protocol} for uuid ${uuid}:`,
      error
    );
    return { errorCode: ERROR_CODES.INTERNAL_SERVER_ERROR };
  }
}

protocolRoutes.post("/", async (req, res, next) => {
  try {
    const { uuid, protocol, data } = req.body;

    if (!uuid || !protocol) {
      return res.status(ERROR_CODES.BAD_REQUEST).json({
        errorCode: ERROR_CODES.BAD_REQUEST,
      });
    }

    const resJson = await handleProtocol(protocol, uuid, data);

    res.json({
      protocol: protocol + 1,
      errorCode: resJson.errorCode,
      uuid,
      ...(resJson.data && { data: resJson.data }),
    });
  } catch (error) {
    next(error);
  }
});

protocolRoutes.use(errorHandler);

module.exports = protocolRoutes;
