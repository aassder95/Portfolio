const fs = require("fs");
const path = require("path");

const loadTemplates = () => {
  const templates = {};
  const templateFiles = ["Enemy", "FishLevel", "Fish", "Shop", "Stage", "Text"];

  templateFiles.forEach((file) => {
    const filePath = path.join(__dirname, `../template/${file}Template.json`);
    const jsonData = fs.readFileSync(filePath, "utf8");
    templates[file] = JSON.parse(jsonData);
  });

  return templates;
};

const templates = loadTemplates();

const templateUtils = {
  getTemplates: (template) => {
    return templates[template];
  },

  getTemplateById: (template, id) => {
    return templateUtils
      .getTemplates(template)
      .find((template) => template.Id === id);
  },

  getShopTemplate: (id) => {
    return templateUtils.getTemplateById("Shop", id);
  },

  getStageTemplate: (type, stage) => {
    return templateUtils
      .getTemplates("Stage")
      .find((template) => template.Type === type && template.Stage === stage);
  },

  getFishLevelTemplate: (level) => {
    return templateUtils
      .getTemplates("FishLevel")
      .find((template) => template.Level === level);
  },
};

module.exports = templateUtils;
