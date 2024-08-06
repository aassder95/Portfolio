const { DataTypes } = require("sequelize");
const sequelize = require("../config/database");

const Deck = sequelize.define(
  "Deck",
  {
    idx: {
      type: DataTypes.INTEGER,
      autoIncrement: true,
      primaryKey: true,
      allowNull: false,
    },
    user_uuid: {
      type: DataTypes.STRING(36),
      unique: true,
      allowNull: false,
    },
    template_id1: {
      type: DataTypes.INTEGER,
      allowNull: false,
    },
    template_id2: {
      type: DataTypes.INTEGER,
      allowNull: false,
    },
    template_id3: {
      type: DataTypes.INTEGER,
      allowNull: false,
    },
    template_id4: {
      type: DataTypes.INTEGER,
      allowNull: false,
    },
    template_id5: {
      type: DataTypes.INTEGER,
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
    tableName: "deck",
    timestamps: false,
  }
);

module.exports = Deck;
