const fs = require("fs");
const path = require("path");
const UserPurchases = require("../models/UserPurchases");
const templateUtils = require("../utils/templates");

const outputPath = path.join(__dirname, "../output/shopRandCard.json");

const getTemplates = () => {
  const shopTemplates = templateUtils.getTemplates("Shop");
  if (!shopTemplates || !Array.isArray(shopTemplates)) {
    throw new Error(
      "Invalid shop templates: templates not found or not an array"
    );
  }
  return shopTemplates;
};

const normalizeRates = (templates) => {
  const totalRate = templates.reduce((sum, template) => sum + template.Rate, 0);
  return templates.map((template) => ({
    ...template,
    Rate: (template.Rate / totalRate) * 100,
  }));
};

const createRanges = (templates) => {
  let sumRate = 0;
  return templates.map((template) => {
    const start = sumRate;
    sumRate += template.Rate;
    return { start, end: sumRate, template };
  });
};

const getRandomItem = (ranges) => {
  const rand = Math.random() * 100;
  return ranges.find((range) => rand >= range.start && rand < range.end)
    .template;
};

const selectRandomItems = (ranges) => {
  const randItems = new Set();
  while (randItems.size < 8) {
    randItems.add(getRandomItem(ranges));
  }
  return Array.from(randItems);
};

const saveResult = (result) => {
  fs.writeFileSync(outputPath, JSON.stringify(result, null, 2));
  console.log("Selected items saved to", outputPath);
};

const clearRandCardIds = async () => {
  await UserPurchases.update({ rand_card_ids: [] }, { where: {} });
  console.log("Cleared rand_card_ids for all user purchases.");
};

const runShopRandCardJob = async () => {
  try {
    console.log("Starting shopRandCard job...");
    await clearRandCardIds();
    const shopTemplates = getTemplates();
    const normalizedTemplates = normalizeRates(shopTemplates);
    const ranges = createRanges(normalizedTemplates);
    const result = selectRandomItems(ranges);
    saveResult(result);
    console.log("shopRandCard job completed successfully.");
  } catch (error) {
    console.error("Error in shopRandCard job:", error.message);
  }
};

runShopRandCardJob();
