const { DataTypes } = require("sequelize");
const sequelize = require("../config/database");

const Chat = sequelize.define(
  "Chat",
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
    user_name: {
      type: DataTypes.STRING,
      allowNull: false,
    },
    message: {
      type: DataTypes.TEXT,
      allowNull: false,
    },
    sent_at: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW,
      allowNull: false,
    },
  },
  {
    tableName: "chat",
    timestamps: false,
  }
);

module.exports = Chat;
