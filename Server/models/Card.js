const { DataTypes } = require("sequelize");
const sequelize = require("../config/database");

const Card = sequelize.define(
  "Card",
  {
    idx: {
      type: DataTypes.INTEGER,
      autoIncrement: true,
      primaryKey: true,
      allowNull: false,
    },
    user_uuid: {
      type: DataTypes.STRING(36),
      allowNull: false,
    },
    template_id: {
      type: DataTypes.INTEGER,
      allowNull: false,
    },
    level: {
      type: DataTypes.INTEGER,
      defaultValue: 1,
      allowNull: false,
    },
    cnt: {
      type: DataTypes.INTEGER,
      defaultValue: 0,
      allowNull: false,
    },
    acquired_at: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW,
      allowNull: false,
    },
    updated_at: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW,
      onUpdate: DataTypes.NOW,
      allowNull: false,
    },
  },
  {
    tableName: "card",
    timestamps: false,
  }
);

module.exports = Card;
